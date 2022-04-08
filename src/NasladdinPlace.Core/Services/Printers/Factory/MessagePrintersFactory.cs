using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Printers.Common;

namespace NasladdinPlace.Core.Services.Printers.Factory
{
    public class MessagePrintersFactory : IMessagePrintersFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<Type, Type> _messagePrinterTypeByEntityTypeDictionary;

        public MessagePrintersFactory(IServiceProvider serviceProvider)
        {   
            _serviceProvider = serviceProvider;
            _messagePrinterTypeByEntityTypeDictionary = CreateMessagePrinterTypeByEntityTypeDictionary();
        }
        
        public IMessagePrinter<T> CreatePrinterFor<T>()
        {
            if (!TryCreateMessagePrinterFor<T>(out var messagePrinter))
            {
                throw new NotImplementedException($"Could not create printer for type {typeof(T).FullName}.");
            }

            return messagePrinter;
        }

        private bool TryCreateMessagePrinterFor<T>(out IMessagePrinter<T> messagePrinter)
        {
            messagePrinter = null;
            
            var messagePrinterType = _messagePrinterTypeByEntityTypeDictionary[typeof(T)];

            if (messagePrinterType == null)
                return false;

            try
            {
                var messagePrinterAsObject = CreateMessagePrinterInstanceFromType(messagePrinterType);
                messagePrinter = messagePrinterAsObject as IMessagePrinter<T>;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private object CreateMessagePrinterInstanceFromType(Type messagePrinterType)
        {
            var printerConstructors = messagePrinterType.GetConstructors();
            var printerConstructorParameters = printerConstructors[0].GetParameters();

            if (!printerConstructorParameters.Any())
                return Activator.CreateInstance(messagePrinterType);

            var resolvedConstructorParameters = printerConstructorParameters.Select(
                p =>
                {
                    var parameterType = p.ParameterType;

                    if (!IsMessagePrinterType(parameterType))
                        return _serviceProvider.GetRequiredService(parameterType);
                    
                    var entityType = GetEntityTypeFromMessagePrinterType(parameterType);
                    var constructorMessagePrinterType = _messagePrinterTypeByEntityTypeDictionary[entityType];
                    return CreateMessagePrinterInstanceFromType(constructorMessagePrinterType);

                })
                .ToArray();
            
            return Activator.CreateInstance(messagePrinterType, resolvedConstructorParameters);
        }

        private static IDictionary<Type, Type> CreateMessagePrinterTypeByEntityTypeDictionary()
        {
            var messagePrinterTypes = GetMessagePrinterTypes();
            
            var messagePrinterTypeByEntityTypeDictionary = messagePrinterTypes.ToImmutableDictionary(
                GetEntityTypeFromMessagePrinterType,
                t => t
            );
            return messagePrinterTypeByEntityTypeDictionary;
        }

        private static IEnumerable<Type> GetMessagePrinterTypes()
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var messagePrinters = callingAssembly.GetTypes().Where(IsNonAbstractMessagePrinterClass).ToImmutableList();
            return messagePrinters;
        }

        private static bool IsNonAbstractMessagePrinterClass(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   type.GetInterfaces().Any(IsMessagePrinterType);
        }

        private static bool IsMessagePrinterType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessagePrinter<>);
        }

        private static Type GetEntityTypeFromMessagePrinterType(Type messagePrinterType)
        {
            var messagePrinterTypeGenericInterface = IsMessagePrinterType(messagePrinterType) 
                ? messagePrinterType 
                : messagePrinterType.GetInterfaces().Single(IsMessagePrinterType);

            var messagePrinterInterfaceGenericArguments =
                messagePrinterTypeGenericInterface.GetGenericArguments();
            return messagePrinterInterfaceGenericArguments.Single();
        }
    }
}
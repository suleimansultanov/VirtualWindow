using System;
using NasladdinPlace.Logging.PurchaseInitiating.Contracts;
using NasladdinPlace.Utilities.EnumHelpers;
using Newtonsoft.Json;

namespace NasladdinPlace.Logging.PurchaseInitiating
{
    public class PurchaseInitiationLogger : IPurchaseInitiationLogger
    {
        private const string MessageHeader = "Purchase initiation";
        private const string Start = "Start";
        private const string End = "End";
        private const string Initiating = "Initiating";
        private const string Result = "Result";

        private readonly ILogger _logger;

        public PurchaseInitiationLogger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public void LogStart(PurchaseInitiationPhase phase, string objectName = null, object initiatingObject = null)
        {
            _logger.LogInfo(CreateMessage(phase, true, objectName, initiatingObject));
        }

        public void LogFinish(PurchaseInitiationPhase phase, string objectName = null, object resultObject = null)
        {
            _logger.LogInfo(CreateMessage(phase, false, objectName, resultObject));
        }

        private string CreateMessage(
            PurchaseInitiationPhase phase,
            bool isStartOfPhase,
            string objectName = null,
            object obj = null)
        {
            var objectInfo = string.Empty;
            if (objectName != null && obj != null)
            {
                var objectDescription = isStartOfPhase ? Initiating : Result;
                objectInfo = $"{objectDescription} object: {SerializeObject(objectName, obj)}";
            }

            var phaseDescription = isStartOfPhase ? Start : End;
            var message = $"{MessageHeader}. {phaseDescription} of {phase.GetDescription()}. {objectInfo}";
            return message;
        }

        private string SerializeObject(string objectName, object obj)
        {
            try
            {
                var serializationSettings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                var objJson = JsonConvert.SerializeObject(obj, serializationSettings);
                return $"{objectName}:{Environment.NewLine}{objJson}";
            }
            catch (Exception e)
            {
                return $"Error while serializing object {objectName}: {e}";
            }
        }
    }
}
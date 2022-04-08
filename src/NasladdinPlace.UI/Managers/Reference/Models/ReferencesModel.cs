using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.UI.Managers.Reference.Interfaces;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    /// <summary>
    /// Модель для хранения данных одного справочника
    /// </summary>
    public class ReferencesModel
    {
        /// <summary>
        /// Имя источника данных
        /// </summary>
        public string ReferenceType { get; set; }

        /// <summary>
        /// Данные для выбора
        /// </summary>
        public List<ReferenceItemModel> Data { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReferencesModel()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="referenceType"></param>
        public ReferencesModel(string referenceType)
        {
            ReferenceType = referenceType;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="referenceType"></param>
        /// <param name="entities"></param>
        /// <param name="notSort"></param>
        public ReferencesModel(string referenceType, IEnumerable<IComboboxViewModel> entities, bool notSort) : this(referenceType)
        {
            Data = notSort
                ? entities.Select(e => new ReferenceItemModel(e)).ToList()
                : entities.Select(e => new ReferenceItemModel(e)).OrderBy(r => r.Text).ToList();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="referenceType"></param>
        /// <param name="entities"></param>
        /// <param name="notSort"></param>
        public ReferencesModel(string referenceType, IEnumerable<IDependencyComboboxViewModel> entities, bool notSort) : this(referenceType)
        {
            Data = notSort
                ? entities.Select(e => new ReferenceItemModel(e)).ToList()
                : entities.Select(e => new ReferenceItemModel(e)).OrderBy(r => r.Text).ToList();
        }

        /// <summary>
        /// Создать источник данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="notSort"></param>
        /// <returns></returns>
        public static ReferencesModel Create<T>(IEnumerable<T> entities, bool notSort = false) where T : IComboboxViewModel
        {
            var type = typeof(T);
            return Create(type.FullName, entities, notSort);
        }

        /// <summary>
        /// Создать источник данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceName"></param>
        /// <param name="entities"></param>
        /// <param name="notSort"></param>
        /// <returns></returns>
        public static ReferencesModel Create<T>(string sourceName, IEnumerable<T> entities, bool notSort = false) where T : IComboboxViewModel
        {
            var type = typeof(T);
            return typeof(IDependencyComboboxViewModel).IsAssignableFrom(type)
                ? new ReferencesModel(sourceName, entities.Cast<IDependencyComboboxViewModel>(), notSort)
                : new ReferencesModel(sourceName, entities.Cast<IComboboxViewModel>(), notSort);
        }
    }
}

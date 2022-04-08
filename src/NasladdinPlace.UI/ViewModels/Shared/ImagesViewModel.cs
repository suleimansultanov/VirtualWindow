using System.Collections.Generic;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.ViewModels.Shared
{
    public class ImagesViewModel
    {
        public IEnumerable<IImage> Images { get; }
        public ICommonHandbook Resource { get; }
        public string DeletionSelector { get; }
        public string AdditionController { get; }
        public string AdditionAction { get; }

        public ImagesViewModel(
            IEnumerable<IImage> images, 
            ICommonHandbook resource,
            string deletionSelector,
            string additionController,
            string additionAction)
        {
            Images = images;
            Resource = resource;
            DeletionSelector = deletionSelector;
            AdditionController = additionController;
            AdditionAction = additionAction;
        }
    }
}
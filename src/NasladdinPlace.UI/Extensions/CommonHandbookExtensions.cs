using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Extensions
{
    public static class CommonHandbookExtensions
    {
        public static SelectList ToSelectList(this IEnumerable<ICommonHandbook> commonHandbooks)
        {
            return new SelectList(commonHandbooks, nameof(ICommonHandbook.Id), nameof(ICommonHandbook.Name));
        }

        public static SelectList ToSelectList(this IEnumerable<Core.Models.ICommonHandbook> commonHandbooks)
        {
            return new SelectList(commonHandbooks, nameof(ICommonHandbook.Id), nameof(ICommonHandbook.Name));
        }
    }
}

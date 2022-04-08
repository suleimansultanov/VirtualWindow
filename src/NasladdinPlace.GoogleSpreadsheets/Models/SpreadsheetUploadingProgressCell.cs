using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Spreadsheets.Models
{
    public class SpreadsheetUploadingProgressCell
    {
        public string Note { get; }
        public string Text { get; }
        public Color BackgroundColor { get; }

        private SpreadsheetUploadingProgressCell(string text, string note, Color backgroundColor)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));
            if(note == null)
                throw new ArgumentNullException(nameof(note));

            Note = note;
            Text = text;
            BackgroundColor = backgroundColor;
           
        }

        public static SpreadsheetUploadingProgressCell Create(SpreadsheetUploadingProgress uploadingProgress, string cellText)
        {
            return uploadingProgress.IsFinished
                ? Finish(cellText, uploadingProgress.FinishedAt)
                : InProgress(cellText, uploadingProgress.StartedAt, uploadingProgress.UploadingProgressInPercents);
        }

        private static SpreadsheetUploadingProgressCell InProgress(string cellText, DateTime startTime, double uploadingProgressInPercents)
        {
            var uploadingStartTimeMessage = $"Выгрузка началась {GetUploadingProcessTimeStamp(startTime)}{Environment.NewLine}";
            string note = $"{uploadingStartTimeMessage}{GetUploadingProgressPercentsMessage(uploadingProgressInPercents)}";
            return new SpreadsheetUploadingProgressCell(cellText, note, CellColors.TaskInProgress());
        }

        private static SpreadsheetUploadingProgressCell Finish(string cellText, DateTime finishTime)
        {
            var note = $"Выгрузка завершилась {GetUploadingProcessTimeStamp(finishTime)}{Environment.NewLine}";
            return new SpreadsheetUploadingProgressCell(cellText, note, CellColors.TaskCompleted());
        }

        private static string GetUploadingProcessTimeStamp(DateTime dateTime)
        {
            dateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime);
            return $"{SharedDateTimeConverter.ConvertDatePartToString(dateTime)} в {SharedDateTimeConverter.ConvertTimePartToString(dateTime)}";
        }

        private static string GetUploadingProgressPercentsMessage(double uploadingPercentage)
        {
            return $"Завершено на {uploadingPercentage:P0}";
        }
    }
}
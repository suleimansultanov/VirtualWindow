using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.PosLogs;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public class PosLogsController : WsController
    {

        public PosLogsController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }

        public async Task Upload(PosLogDto posLog)
        {
            if (!posLog.PosId.HasValue)
                return;

            var dbPosLog = new PosLog(posId: posLog.PosId.Value,
                                      logType: posLog.LogType,
                                      fileName: posLog.FileName,
                                      fileContent: Convert.FromBase64String(posLog.FileContent));

            //TODO: Переделать работу с логами витрин вне БД. 
            //await ExecuteAsync(async unitOfWork =>
            //{
            //    var posLogRepository = unitOfWork.GetRepository<PosLog>();
            //    posLogRepository.Add(dbPosLog);

            //    await unitOfWork.CompleteAsync();
            //});
        }
    }
}

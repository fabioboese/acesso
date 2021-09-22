using Acesso.Account.Domain.Interfaces;
using Acesso.Account.Domain.ViewModel;
using Acesso.Account.Domain.ViewModel.Request;
using Acesso.Account.Domain.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acesso.Account.Service.Controllers
{
    [Route("api")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private ITransferTransactionApp transferTransactionApp;

        public TransferController(ITransferTransactionApp transferTransactionApp)
        {
            this.transferTransactionApp = transferTransactionApp;
        }

        [HttpPost]
        [Route("fund-transfer")]
        public async Task<Guid> TransferAsync([FromBody]TransferOperationRequest request)
        {
            Guid id = await transferTransactionApp.TransferAsync(
                request.accountOrigin,
                request.accountDestination,
                request.value);

            return id;
        }

        [HttpGet]
        [Route("fund-transfer/{transactionId}")]
        public async Task<TransferStatusResponse> GetStatusAsync([FromRoute]Guid transactionId)
        {
            var transfer = await transferTransactionApp.GetStatusAsync(transactionId);
            return new TransferStatusResponse
            {
                Status = transfer.Status.ToString(),
                Message = transfer.errorMessage
            };
        }
    }
}

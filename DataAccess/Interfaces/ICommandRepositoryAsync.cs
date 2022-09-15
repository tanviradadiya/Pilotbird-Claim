using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ICommandRepositoryAsync
    {
        Task<TResult> ExecuteCommandScalarAsync<TResult>
        (
            string commandTextToExecute,
            CommandType commandType,
            object parameters = null,
            CancellationToken? cancellationToken = null
        );

        Task<int> ExecuteCommandAsync
        (
            string commandTextToExecute,
            CommandType commandType,
            object parameters = null,
            CancellationToken? cancellationToken = null
        );
    }
}

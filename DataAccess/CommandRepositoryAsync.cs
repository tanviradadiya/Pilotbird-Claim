using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace DataAccess
{
    public class CommandRepositoryAsync : ICommandRepositoryAsync
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommandRepositoryAsync(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        private async Task<TResult> ExecuteCommandWithOptionalConnectionDispose<TResult>
            (
                IDbConnection connection,
                Func<IDbConnection, Task<TResult>> commandResultGenerator
            ) => await commandResultGenerator(connection).ConfigureAwait(false);

        public async Task<TResult> ExecuteCommandScalarAsync<TResult>
                (
                     string commandTextToExecute,
                     CommandType commandType = CommandType.Text,
                     object parameters = null,
                     CancellationToken? cancellationToken = null
                ) => await ExecuteCommandWithOptionalConnectionDispose
            (
                _unitOfWork?.Connection,
                (conn) => conn.ExecuteScalarAsync<TResult>
                (
                    new CommandDefinition
                    (
                        commandTextToExecute,
                        parameters,
                        transaction: _unitOfWork?.Transaction,
                        commandType: commandType,
                        commandTimeout: 1000,
                        cancellationToken: cancellationToken ?? CancellationToken.None
                    )
                )
            )
            .ConfigureAwait(false);

        public async Task<int> ExecuteCommandAsync
            (
                 string commandTextToExecute,
                 CommandType commandType = CommandType.Text,
                 object parameters = null,
                 CancellationToken? cancellationToken = null
            ) => await ExecuteCommandWithOptionalConnectionDispose
            (
                _unitOfWork?.Connection,
                (conn) => conn.ExecuteAsync
                (
                    new CommandDefinition
                    (
                        commandTextToExecute,
                        parameters,
                        transaction: _unitOfWork?.Transaction,
                        commandType: commandType,
                        commandTimeout: 1000,
                        cancellationToken: cancellationToken ?? CancellationToken.None
                    )
                )
            ).ConfigureAwait(false);
    }
}

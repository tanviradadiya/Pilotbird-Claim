using System;
using System.Data;

namespace DataAccess
{
    public interface IUnitOfWork
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }

        Guid UnitOfWorkId { get; }

        void Begin();

        void Commit();

        void Rollback();
    }
}

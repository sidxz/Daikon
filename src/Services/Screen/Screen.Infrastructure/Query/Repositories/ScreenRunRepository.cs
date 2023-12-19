using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Entities;
using Screen.Domain.EntityRevisions;

namespace Screen.Infrastructure.Query.Repositories
{
    public class ScreenRunRepository : IScreenRunRepository
    {
        public Task CreateScreenRun(ScreenRun screenRun)
        {
            throw new NotImplementedException();
        }

        public Task DeleteScreenRun(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteScreenRunsByScreenId(Guid screenId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ScreenRun>> GetScreenRunList()
        {
            throw new NotImplementedException();
        }

        public Task<List<ScreenRun>> GetScreenRunListByScreenId(Guid screenId)
        {
            throw new NotImplementedException();
        }

        public Task<ScreenRunRevision> GetScreenRunRevisions(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<ScreenRun> ReadScreenRunById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateScreenRun(ScreenRun screenRun)
        {
            throw new NotImplementedException();
        }
    }
}
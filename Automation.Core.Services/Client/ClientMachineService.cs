using System;
using System.Collections.Generic;
using System.Linq;
using Automation.Core.Data;
using Automation.Core.Domain.Client;
using Automation.Core.Events;
using Automation.Core.Services.Events;
using Automation.Extensions;

namespace Automation.Core.Services.Client
{
    public class ClientMachineService : IClientMachineService
    {
        private readonly IRepository<ClientMachineExecutionData> _clientMachineExecutionDataRepository;
        private readonly IRepository<ClientMachine> _clientMachineRepository;
        private readonly IEventPublisher _eventPublisher;

        public ClientMachineService(IRepository<ClientMachine> clientMachineRepository,
            IRepository<ClientMachineExecutionData> clientMachineExecutionDataRepository, IEventPublisher eventPublisher)
        {
            _clientMachineRepository = clientMachineRepository;
            _clientMachineExecutionDataRepository = clientMachineExecutionDataRepository;
            _eventPublisher = eventPublisher;
        }

        public ClientMachineExecutionData GetClientMachineExecutionDataByClientId(int id)
        {
            var clientMachine = _clientMachineRepository.GetById(id);
            if (clientMachine == null)
                return null;

            var result =
                _clientMachineExecutionDataRepository.Table.FirstOrDefault(c => c.ClientMachineId == clientMachine.Id);

            clientMachine.LastConnectionOn = DateTime.Now;
            _clientMachineRepository.Update(clientMachine);

            return result;
        }

        public IEnumerable<ClientMachine> GetAllClientMachines(bool showDisabled = true)
        {
            var machines = _clientMachineRepository.Table.Where(m => !m.Deleted);
            return showDisabled ? machines : machines.Where(m => m.Active);
        }

        public void InsertClientMachine(ClientMachine clientMachine)
        {
            Guard.NotNull(clientMachine, "clientMachine");

            _clientMachineRepository.Insert(clientMachine);
            _eventPublisher.EntityInserted(clientMachine);
        }

        public ClientMachine GetClientMachineById(int id)
        {
            var query = _clientMachineRepository.Table.Where(c => !c.Deleted && c.Id == id);

            return query.OrderBy(x => x.Id).FirstOrDefault();
        }

        public void UpdateClientMachine(ClientMachine clientMachine)
        {
            Guard.NotNull(clientMachine, "clientMachine");

            _clientMachineRepository.Update(clientMachine);
            _eventPublisher.EntityUpdated(clientMachine);
        }

        public void DeleteClientMachine(ClientMachine clientMachine)
        {
            Guard.NotNull(clientMachine, "clientMachine");

            clientMachine.Deleted = true;
            _clientMachineRepository.Update(clientMachine);

            _eventPublisher.EntityDeleted(clientMachine);
        }
    }
}
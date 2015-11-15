using System.Collections.Generic;
using Automation.Core.Domain.Client;

namespace Automation.Core.Services.Client
{
    public interface IClientMachineService
    {
        ClientMachineExecutionData GetClientMachineExecutionDataByClientId(int id);
        IEnumerable<ClientMachine> GetAllClientMachines(bool showDisabled = false);
        void InsertClientMachine(ClientMachine clientMachine);
        ClientMachine GetClientMachineById(int id);
        void UpdateClientMachine(ClientMachine clientMachine);
        void DeleteClientMachine(ClientMachine clientMachine);
    }
}

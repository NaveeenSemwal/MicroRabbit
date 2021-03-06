﻿using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using MicroRabbit.Domain.Core.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Banking.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IEventBus _bus;

        public AccountService(IAccountRepository repository, IEventBus bus)
        {
            _repository = repository;
            _bus = bus;
        }
        public IEnumerable<Account> GetAccounts()
        {
            return _repository.GetAccounts();
        }

        public void Transfer(AccountTransafer transafer)
        {
            var createTransferCommand = new CreateTransferCommand(transafer.FromAccount, transafer.ToAccount, transafer.TransferAmount);
            _bus.SendCommand(createTransferCommand);
        }
    }
}

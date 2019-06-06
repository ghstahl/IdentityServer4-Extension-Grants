using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Stores
{
    /// <summary>
    /// Client store decorator for running runtime configuration validation checks
    /// </summary>
    public class ValidatingClientStoreExtra<T> : IClientStoreExtra
        where T : IClientStoreExtra
    {
        private readonly IClientStoreExtra _inner;
        private readonly IClientConfigurationValidator _validator;
        private readonly IEventService _events;
        private readonly ILogger<ValidatingClientStoreExtra<T>> _logger;
        private readonly string _validatorType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingClientStore{T}" /> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public ValidatingClientStoreExtra(T inner, IClientConfigurationValidator validator, IEventService events, ILogger<ValidatingClientStoreExtra<T>> logger)
        {
            _inner = inner;
            _validator = validator;
            _events = events;
            _logger = logger;

            _validatorType = validator.GetType().FullName;
        }

        /// <summary>
        /// Finds a client by id (and runs the validation logic)
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client or an InvalidOperationException
        /// </returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _inner.FindClientByIdAsync(clientId);

            if (client != null)
            {
                _logger.LogTrace("Calling into client configuration validator: {validatorType}", _validatorType);

                var context = new ClientConfigurationValidationContext(client);
                await _validator.ValidateAsync(context);

                if (context.IsValid)
                {
                    _logger.LogDebug("client configuration validation for client {clientId} succeeded.", client.ClientId);
                    return client;
                }
                else
                {
                    _logger.LogError("Invalid client configuration for client {clientId}: {errorMessage}", client.ClientId, context.ErrorMessage);
                    await _events.RaiseAsync(new InvalidClientConfigurationEvent(client, context.ErrorMessage));

                    return null;
                }
            }

            return null;
        }

        public Task<List<string>> GetAllClientIdsAsync()
        {
            return _inner.GetAllClientIdsAsync();
        }

        public Task<List<ClientExtra>> GetAllClientsAsync()
        {
            return _inner.GetAllClientsAsync();
        }

        public Task<List<string>> GetAllEnabledClientIdsAsync()
        {
            return _inner.GetAllEnabledClientIdsAsync();
        }
    }
}

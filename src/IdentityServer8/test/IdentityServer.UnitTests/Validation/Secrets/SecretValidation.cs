﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer8;
using IdentityServer8.Configuration;
using IdentityServer8.Models;
using IdentityServer8.Stores;
using IdentityServer8.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class SecretValidation
    {
        private const string Category = "Secrets - Secret Validator";

        private ISecretValidator _hashedSecretValidator = new HashedSharedSecretValidator(new Logger<HashedSharedSecretValidator>(new LoggerFactory()));
        private IClientStore _clients = new InMemoryClientStore(ClientValidationTestClients.Get());
        private SecretValidator _validator;
        private IdentityServerOptions _options = new IdentityServerOptions();

        public SecretValidation()
        {
            _validator = new SecretValidator(
                new StubClock(),
                new[] { _hashedSecretValidator }, 
                new Logger<SecretValidator>(new LoggerFactory()));
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Single_Secret()
        {
            var clientId = "single_secret_hashed_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Credential_Type()
        {
            var clientId = "single_secret_hashed_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = "invalid"
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Multiple_Secrets()
        {
            var clientId = "multiple_secrets_hashed";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeTrue();

            secret.Credential = "foobar";
            result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeTrue();

            secret.Credential = "quux";
            result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeTrue();

            secret.Credential = "notexpired";
            result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Single_Secret()
        {
            var clientId = "single_secret_hashed_no_expiration";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "invalid",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Expired_Secret()
        {
            var clientId = "multiple_secrets_hashed";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "expired",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Multiple_Secrets()
        {
            var clientId = "multiple_secrets_hashed";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "invalid",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Client_with_no_Secret_Should_Fail()
        {
            var clientId = "no_secret_client";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };
            
            var result = await _validator.ValidateAsync(client.ClientSecrets, secret);
            result.Success.Should().BeFalse();
        }
    }
}
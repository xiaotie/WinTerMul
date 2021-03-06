﻿using System;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WinTerMul.Common
{
    internal class WinTerMulConfiguration : IWinTerMulConfiguration
    {
        private readonly IConfiguration _configuration;

        public WinTerMulConfiguration(IConfiguration configuration)
        {
            Validate(configuration);

            _configuration = configuration;
        }

        public LogLevel LogLevel => (LogLevel)Enum.Parse(typeof(LogLevel), _configuration["LogLevel"]);
        public string LogPath => _configuration["LogPath"];
        public string PrefixKey => _configuration["PrefixKey"].ToLower();
        public char SetNextTerminalActiveKey => _configuration["SetNextTerminalActiveKey"][0];
        public char SetPreviousTerminalActive => _configuration["SetPreviousTerminalActive"][0];
        public char VerticalSplitKey => _configuration["VerticalSplitKey"][0];
        public char ClosePaneKey => _configuration["ClosePaneKey"][0];

        private void Validate(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ValidateLogLevel(configuration);
            ValidateLogPath(configuration);
            ValidatePrefixKey(configuration);
            ValidateKeyBinding(configuration, nameof(SetNextTerminalActiveKey));
            ValidateKeyBinding(configuration, nameof(SetPreviousTerminalActive));
            ValidateKeyBinding(configuration, nameof(VerticalSplitKey));
            ValidateKeyBinding(configuration, nameof(ClosePaneKey));
        }

        private void ValidateLogLevel(IConfiguration configuration)
        {
            if (!Enum.TryParse<LogLevel>(configuration["LogLevel"], out _))
            {
                throw new ArgumentException($"Invalid or missing {nameof(LogLevel)} configuration.");
            }
        }

        private void ValidateLogPath(IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration["LogPath"]))
            {
                throw new ArgumentException($"{nameof(LogPath)} not specified.");
            }
        }

        private void ValidatePrefixKey(IConfiguration configuration)
        {
            var prefixKey = configuration["PrefixKey"]?.ToLower();
            var allowedCharacters = GetAllowedCharacters();

            var isValidPrefixKey = prefixKey?.Length == 3 &&
                prefixKey.StartsWith("c-") &&
                allowedCharacters.Contains(prefixKey[2]);

            if (!isValidPrefixKey)
            {
                throw new ArgumentException($"Invalid {nameof(PrefixKey)} configuration.");
            }
        }

        private void ValidateKeyBinding(IConfiguration configuration, string property)
        {
            var allowedCharacters = GetAllowedCharacters();
            var @char = configuration[property]?.FirstOrDefault();
            if (!@char.HasValue || !allowedCharacters.Contains(@char.Value))
            {
                throw new ArgumentException($"Invalid {property} configuration.");
            }
        }

        private char[] GetAllowedCharacters() => Enumerable.Range('a', 26).Select(x => (char)x).ToArray();
    }
}

# Security Policy

## Supported Versions

Use this section to tell people about which versions of your project are
currently being supported with security updates.

| Version | Supported          |
| ------- | ------------------ |
| 5.1.x   | :white_check_mark: |
| 5.0.x   | :x:                |
| 4.0.x   | :white_check_mark: |
| < 4.0   | :x:                |

## Reporting a Vulnerability

Use this section to tell people how to report a vulnerability.

Tell them where to go, how often they can expect to get an update on a
reported vulnerability, what to expect if the vulnerability is accepted or
declined, etc.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Utilities
{
    public class DatabaseParameterWrappers
    {
        /// <summary>
        /// For Input Parameters
        /// </summary>
        /// <param name="_ParameterValue"></param>
        /// <param name="_DbParameterType"></param>
        /// <param name="_DbParameterDirection"></param>
        public DatabaseParameterWrappers(object _ParameterValue, DbType _DbParameterType = DbType.String, int _size = default, ParameterDirection _DbParameterDirection = ParameterDirection.Input)
        {
            ParameterValue = _ParameterValue;
            DbParameterType = _DbParameterType;
            DbParameterDirection = _DbParameterDirection;
            size = _size;
        }

        /// <summary>
        /// For Output Parameters
        /// </summary>
        /// <param name="_DbParameterType"></param>
        /// <param name="_DbParameterDirection"></param>
        public DatabaseParameterWrappers(DbType _DbParameterType = DbType.String, int _size = default, ParameterDirection _DbParameterDirection = ParameterDirection.Output)
        {
            DbParameterType = _DbParameterType;
            DbParameterDirection = _DbParameterDirection;
            size = _size;
        }

        public object ParameterValue { get; set; }
        public DbType DbParameterType { get; set; } = DbType.String;
        public ParameterDirection DbParameterDirection { get; set; } = ParameterDirection.Input;
        public int size { get; set; }
    }
}

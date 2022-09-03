﻿using NoodleManagerX.Models.Mods;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace NoodleManagerX.Mods
{
    public class ModDependencyGraph
    {
        public enum ResolvedState
        {
            UNRESOLVED,
            RESOLVING,
            RESOLVED,
            ERROR_MISSING_DEP,
            ERROR_VERSION_MISMATCH,
        }

        public ResolvedState State { get; private set; } = ResolvedState.UNRESOLVED;
        public string Message { get; private set; } = "";

        // ModInfo.id, ModVersion
        public Dictionary<string, ModVersion> ResolvedVersions { get; private set; } = new();

        // ModInfo.id, ModInfo
        private Dictionary<string, ModInfo> _mods = new();


        public void AddMod(ModInfo mod)
        {
            _mods.Add(mod.Id, mod);
        }

        private bool IsVersionValid(ModVersion version, Dictionary<string, ModVersion> currentVersions)
        {
            // Dependencies must be present
            foreach (var dep in version.Dependencies)
            {
                if (!currentVersions.ContainsKey(dep.Id))
                {
                    return false;
                }
            }

            return true;
        }

        public void Resolve()
        {
            // Reset resolved versions until resolution is finished
            ResolvedVersions = new();
            State = ResolvedState.RESOLVING;

            var finalVersions = new Dictionary<string, ModVersion>();

            // Add base mods
            foreach (var mod in _mods.Values)
            {
                ModVersion selectedVersion = null;
                foreach (var version in mod.Versions)
                {
                    if (IsVersionValid(version, finalVersions))
                    {
                        if (selectedVersion == null || 
                            version.Version.ComparePrecedenceTo(selectedVersion.Version) > 0)
                        {
                            // version > selectedVersion. Choose max valid
                            selectedVersion = version;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Ignoring invalid version {version.Version} for mod {mod.Id}");
                    }
                }
                if (selectedVersion == null)
                {
                    Console.WriteLine($"Valid version for mod {mod.Id} not found");
                    State = ResolvedState.ERROR_MISSING_DEP;
                    return;
                }
                finalVersions.Add(mod.Id, selectedVersion);
            }

            /*

            // Check dependencies
            foreach (var mod in _modVersions.Values)
            {
                foreach (var depInfo in mod.Dependencies)
                {
                    if (finalVersions.ContainsKey(depInfo.Id))
                    {
                        // Dependency exists. Check version range
                        var existingModInfo = finalVersions[depInfo.Id];
                        if (IsVersionInRange(existingModInfo.Version, depInfo.MinVersion, depInfo.MaxVersion))
                        {
                            Console.WriteLine($"Dependency {depInfo.Id} already defined. Version is in range");
                        }
                        else
                        {
                            Message = $"Dependency {depInfo.Id} already defined at version {existingModInfo.Version}" +
                                $" outside of required range {depInfo.MinVersion} to {depInfo.MaxVersion}";
                            State = ResolvedState.ERROR_VERSION_MISMATCH;
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Dependency {depInfo.Id} not defined in list");
                        State = ResolvedState.ERROR_MISSING_DEP;
                        return;
                    }
                }
            }*/

            State = ResolvedState.RESOLVED;
            ResolvedVersions = finalVersions;
        }

        private static bool IsVersionInRange(SemVersion version, SemVersion lowInclusive, SemVersion highInclusive)
        {
            if (version.ComparePrecedenceTo(lowInclusive) == -1)
            {
                return false;
            }

            if (highInclusive != null && version.ComparePrecedenceTo(highInclusive) == 1)
            {
                return false;
            }

            return true;
        }
    }
}

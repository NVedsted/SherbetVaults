﻿using Cysharp.Threading.Tasks;
using Rocket.API;
using RocketExtensions.Models;
using RocketExtensions.Plugins;

namespace SherbetVaults.Commands
{
    [CommandName("Vault")]
    [CommandInfo("Opens a vault", Syntax: "[Vault Name]")]
    [AllowedCaller(AllowedCaller.Player)]
    public class VaultCommand : RocketCommand
    {
        public override async UniTask Execute(CommandContext context)
        {
            var targetVault = context.Arguments.Get(0, defaultValue: "default", paramName: "Vault Name");

            var vaultConfig = Plugin.GetVaultConfig(targetVault);

            if (vaultConfig == null)
            {
                await context.ReplyKeyAsync("Vault_Fail_NotFound", targetVault);
                return;
            }

            if (!await context.CheckPermissionAsync(vaultConfig.Permission))
            {
                await context.ReplyKeyAsync("Vault_Fail_NoPermission", vaultConfig.VaultID);
                return;
            }

            var vault = await Plugin.VaultManager.GetVault(context.PlayerID, targetVault);

            if (vault == null)
            {
                await context.ReplyKeyAsync("Vault_Fail_CannotLoad", targetVault);
                return;
            }

            vault.OpenForPlayer(context.LDMPlayer);
        }

        private new SherbetVaultsPlugin Plugin =>
            base.Plugin as SherbetVaultsPlugin;
    }
}
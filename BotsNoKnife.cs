using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace BotsNoKnife;

public class BotsNoKnife : BasePlugin
{
	public override string ModuleName => "BotsNoKnife";
	public override string ModuleVersion => "1.0";
	public override string ModuleAuthor => "austin";
	public override string ModuleDescription => "Keeps Bots from using the knife";
	public override void Load(bool hotReload)
	{
		Console.WriteLine($"Plugin: {this.ModuleName} - Version: {this.ModuleVersion} by {this.ModuleAuthor} - Loaded.");
		VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Hook(OnWeaponCanAcquire, HookMode.Pre);
	}
	public override void Unload(bool hotReload)
	{
		VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Unhook(OnWeaponCanAcquire, HookMode.Pre);
	}
	public HookResult OnWeaponCanAcquire(DynamicHook hook)
	{
		CCSWeaponBaseVData vdata = VirtualFunctions.GetCSWeaponDataFromKeyFunc.Invoke(-1, hook.GetParam<CEconItemView>(1).ItemDefinitionIndex.ToString());
		if (vdata == null)
			return HookResult.Continue;
		CCSPlayerController client = hook.GetParam<CCSPlayer_ItemServices>(0).Pawn.Value!.Controller.Value!.As<CCSPlayerController>();
		if (client == null || !client.IsValid || !client.PawnIsAlive || !client.IsBot)
			return HookResult.Continue;
		
		if (!"weapon_knife".Contains(vdata.Name))
		//if (vdata.Name  != "weapon_knife")
			return HookResult.Continue;
		hook.SetReturn(AcquireResult.InvalidItem);
		return HookResult.Stop;
	}
}

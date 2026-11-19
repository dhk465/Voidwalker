using System;
using System.IO;
using System.Text.Json;
using Voidwalker.Core;

namespace Voidwalker.Hub;

/// <summary>The hub's shared collection, rules and save file.</summary>
public sealed class Session
{
	/// <summary>Where the save is written.</summary>
	public const string SavePath = "user://save.json";
	/// <summary>Where the data tables are read from.</summary>
	public const string DataPath = "res://src/data/game_data.json";

	/// <summary>The data tables.</summary>
	public GameData Data => Rules.Data;
	/// <summary>The rules over the current collection.</summary>
	public AltarRules Rules { get; private set; }
	/// <summary>The storage status shown to the player.</summary>
	public string SaveNotice { get; private set; } = "";

	private readonly string _path;
	private bool _protected;

	private Session(GameData data, Meta meta, string path) { Rules = new AltarRules(data, meta); _path = path; }

	/// <summary>Loads an existing profile, or starts the demo. Failed reads protect the original file.</summary>
	public static Session Load(GameData data, string path)
	{
		Session session;
		try { session = new Session(data, Meta.FromJson(File.ReadAllText(path)), path); }
		catch (FileNotFoundException) { session = new Session(data, Meta.Demo(), path); }
		catch (Exception e) when (e is IOException or UnauthorizedAccessException or JsonException)
		{
			session = new Session(data, Meta.Demo(), path) { _protected = true, SaveNotice = "SAVE PROTECTED - DEMO CHANGES ARE TEMPORARY" };
		}
		if (session.Rules.EnsureRifts() && !session._protected) session.Save();
		return session;
	}

	/// <summary>Persists the current collection unless the existing save could not be loaded.</summary>
	public bool Save() => !_protected && Write(Rules.Meta);

	private bool Write(Meta meta)
	{
		string temp = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
		try
		{
			File.WriteAllText(temp, meta.ToJson());
			File.Move(temp, _path, overwrite: true);
			SaveNotice = "";
			return true;
		}
		catch (Exception e) when (e is IOException or UnauthorizedAccessException)
		{
			SaveNotice = _protected ? "RESET FAILED - ORIGINAL SAVE PROTECTED" : "SAVE FAILED - CHANGES ARE TEMPORARY";
			return false;
		}
		finally
		{
			try { File.Delete(temp); }
			catch (IOException) { }
			catch (UnauthorizedAccessException) { }
		}
	}

	/// <summary>Explicitly resets to the same playable collection as a first launch.</summary>
	public bool ForgetAll()
	{
		var starter = new AltarRules(Data, Meta.Demo());
		starter.EnsureRifts();
		if (!Write(starter.Meta)) return false;
		Rules = starter;
		_protected = false;
		return true;
	}
}

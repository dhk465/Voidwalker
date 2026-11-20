using System;
using System.Linq;
using Godot;
using Voidwalker.Core;
using Voidwalker.Hub;

namespace Voidwalker.Ui;

/// <summary>
/// The Rift Gate: choose one of the rifts offered (its prevailing nature, omen and boon), the depth, and with the
/// Chosen sigil a first card. Entering the rift explains that the adventure is not ported yet. The rift cards take
/// the body's width and whatever height the control rows at the bottom leave; each shows the hub's rift in small,
/// painted in its nature, when it has the room.
/// </summary>
public partial class GateScreen : StationScreen
{
	private const int Gap = 8, RowH = 30, Line = Ui.SmallSize;
	private const int RiftH = 52, RiftPivotY = 53; // assets/fx/gate-rift-small.json: frame height and pivot

	/// <inheritdoc/>
	protected override string Title => "RIFT GATE";

	/// <inheritdoc/>
	protected override void Build()
	{
		bool chosen = R.HasChosen();
		int ctrlY = BodyH - RowH, chosenY = ctrlY - RowH - 4, cardH = (chosen ? chosenY : ctrlY) - 6;

		var rifts = R.Meta.Rifts ?? new();
		int n = Math.Max(1, rifts.Count), w = (BodyW - (n - 1) * Gap) / n;
		var tintShader = GD.Load<Shader>("res://src/shaders/canvas_env_rift_tint.gdshader");
		int swirlH = cardH - 160 >= RiftH ? RiftH : 0; // the small rift when the card has room for it
		for (int i = 0; i < rifts.Count; i++)
		{
			var r = rifts[i]; int k = i;
			var nb = D.Natures[r.Prevail];
			var colour = new Color(nb.Color);
			bool on = R.Meta.RiftPick == i;
			var card = Ui.Tile(Body, i * (w + Gap), 0, w, cardH, () => { R.PickRift(k); Commit(); }, colour: on ? colour : Pal.Line);
			card.Fill = on ? Pal.Chosen : Pal.Panel;
			if (swirlH > 0)
			{
				var rift = SheetPlayer.Load("res://assets/fx/", "gate-rift-small", new Vector2(w / 2, 6 + RiftPivotY));
				var mat = new ShaderMaterial { Shader = tintShader };
				mat.SetShaderParameter("tint", colour);
				rift.Material = mat;
				card.AddChild(rift);
			}

			int y = 6 + swirlH + 4, textW = w - 12;
			Ui.Text(card, r.Name, 6, y, Pal.Bright, width: textW, align: HorizontalAlignment.Center);
			y += r.Name.Length * Ui.SmallW > textW ? 2 * Line : Line;
			int sigilY = y + 4;
			card.Paint = t => t.DrawTexture(PixelIcons.Sigil(r.Prevail, colour), new Vector2(6, sigilY));
			Ui.Text(card, nb.Name, 19, y + 1, colour); y += Line + 2;
			Ui.Text(card, Ui.FitPx($"Weak to {D.Natures[nb.Opposite].Name}", textW), 6, y, Pal.Soft); y += Line + 2;
			Ui.Text(card, Ui.FitPx(D.Omens[r.Omen].Name, textW), 6, y, Pal.Gold); y += Line + 2;
			Ui.Text(card, Ui.FitPx(D.Boons[r.Boon].Name, textW), 6, y, Pal.Mint); y += Line;
			Ui.Text(card, D.Boons[r.Boon].Says, 6, y, Pal.Mid, width: textW);
		}

		// the Chosen sigil's first card
		if (chosen)
		{
			Ui.Text(Body, "FIRST CARD", 0, chosenY + 7, Pal.Mid);
			Ui.Tile(Body, 92, chosenY, 30, RowH, () => { R.StepChosen(-1); Commit(); }, "<", Pal.Void, big: true);
			var st = AltarRules.Stacks(R.Meta.Loadout).FirstOrDefault(s => s.Sk == R.Meta.Chosen);
			int nameW = Math.Min(240, BodyW - 92 - 30 - 30 - 8);
			if (st != null) Ui.Text(Body, Ui.FitPx(R.CardName(st.Natures, st.Level), nameW), 126, chosenY + 7, new Color(R.CardColor(st.Natures)), width: nameW, align: HorizontalAlignment.Center);
			else Ui.Text(Body, "NONE", 126, chosenY + 7, Pal.Dim, width: nameW, align: HorizontalAlignment.Center);
			Ui.Tile(Body, 130 + nameW, chosenY, 30, RowH, () => { R.StepChosen(1); Commit(); }, ">", Pal.Void, big: true);
		}

		// depth, and the way in
		Ui.Text(Body, "DEPTH", 0, ctrlY + 7, Pal.Bright);
		Ui.Tile(Body, 48, ctrlY, 30, RowH, () => { R.StepDepth(-1); Commit(); }, "-", Pal.Void, R.Meta.Depth > 0, big: true);
		Ui.Text(Body, $"{R.Meta.Depth}", 78, ctrlY + 3, Pal.Gold, big: true, width: 30, align: HorizontalAlignment.Center);
		Ui.Tile(Body, 108, ctrlY, 30, RowH, () => { R.StepDepth(1); Commit(); }, "+", Pal.Void, R.Meta.Depth < R.Meta.MaxDepth, big: true);
		if (R.Meta.Depth > 0) { var (life, v) = R.DepthBonus(); Ui.Text(Body, $"+{life}% LIFE +{v}% VOID", 146, ctrlY + 7, Pal.Mid); }
		bool ready = R.DeckCount() >= R.DeckMin();
		Ui.Tile(Body, BodyW - 200, ctrlY, 200, RowH, Enter, "ENTER THE RIFT", Pal.Mint, ready, big: true);
		if (!ready) Say($"THE POOL NEEDS {R.DeckMin()}", Pal.Warn);
	}

	// Into the rift: the save is written first, then the adventure's scene takes over from the hub.
	private void Enter()
	{
		string why = R.BeginAdventure();
		if (why != "") { Say(why, Pal.Gold); return; }
		S.Save();
		GetTree().ChangeSceneToFile(Voidwalker.Adventure.RiftScene.ScenePath);
	}
}

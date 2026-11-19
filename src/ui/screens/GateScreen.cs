using System;
using System.Linq;
using Godot;
using Voidwalker.Core;

namespace Voidwalker.Ui;

/// <summary>
/// The Rift Gate: choose one of the rifts offered (its prevailing nature, omen and boon), the depth, and with the
/// Chosen sigil a first card. Entering the rift explains that the adventure is not ported yet.
/// </summary>
public partial class GateScreen : StationScreen
{
	private const int CardH = 200, Gap = 8;

	/// <inheritdoc/>
	protected override string Title => "RIFT GATE";

	/// <inheritdoc/>
	protected override void Build()
	{
		var rifts = R.Meta.Rifts ?? new();
		int n = Math.Max(1, rifts.Count), w = (BodyW - (n - 1) * Gap) / n;
		var swirlShader = GD.Load<Shader>("res://src/shaders/canvas_env_gate_swirl.gdshader");
		for (int i = 0; i < rifts.Count; i++)
		{
			var r = rifts[i]; int k = i;
			var nb = D.Natures[r.Prevail];
			var colour = new Color(nb.Color);
			bool on = R.Meta.RiftPick == i;
			var card = Ui.Tile(Body, i * (w + Gap), 0, w, CardH, () => { R.PickRift(k); Commit(); }, colour: on ? colour : Pal.Line);
			card.Fill = on ? Pal.Chosen : Pal.Panel;
			var swirl = new ColorRect { Position = new Vector2(w / 2 - 20, 8), Size = new Vector2(40, 64), MouseFilter = MouseFilterEnum.Ignore };
			var mat = new ShaderMaterial { Shader = swirlShader };
			mat.SetShaderParameter("tint", colour);
			mat.SetShaderParameter("rect_size", swirl.Size);
			swirl.Material = mat;
			card.AddChild(swirl);
			Ui.Text(card, r.Name, 0, 78, Pal.Bright, big: true, width: w, align: HorizontalAlignment.Center);
			card.Paint = t => t.DrawTexture(PixelIcons.Sigil(r.Prevail, colour), new Vector2(10, 101));
			Ui.Text(card, nb.Name, 24, 99, colour);
			Ui.Text(card, $"Weak to {D.Natures[nb.Opposite].Name}", 10, 114, Pal.Soft);
			Ui.Text(card, D.Omens[r.Omen].Name, 10, 132, Pal.Gold);
			Ui.Text(card, D.Boons[r.Boon].Name, 10, 150, Pal.Mint);
			Ui.Text(card, D.Boons[r.Boon].Says, 10, 164, Pal.Mid, width: w - 20);
		}

		// depth
		Ui.Text(Body, "DEPTH", 0, 212, Pal.Bright, big: true);
		Ui.Tile(Body, 50, 210, 24, 22, () => { R.StepDepth(-1); Commit(); }, "-", Pal.Void, R.Meta.Depth > 0, big: true);
		Ui.Text(Body, $"{R.Meta.Depth}", 74, 213, Pal.Gold, big: true, width: 24, align: HorizontalAlignment.Center);
		Ui.Tile(Body, 98, 210, 24, 22, () => { R.StepDepth(1); Commit(); }, "+", Pal.Void, R.Meta.Depth < R.Meta.MaxDepth, big: true);
		if (R.Meta.Depth > 0) { var (life, v) = R.DepthBonus(); Ui.Text(Body, $"+{life}% LIFE  +{v}% VOID", 130, 216, Pal.Mid); }

		// the Chosen sigil's first card
		if (R.HasChosen())
		{
			Ui.Text(Body, "FIRST CARD", 330, 216, Pal.Mid);
			Ui.Tile(Body, 400, 210, 22, 22, () => { R.StepChosen(-1); Commit(); }, "<", Pal.Void);
			var st = AltarRules.Stacks(R.Meta.Loadout).FirstOrDefault(s => s.Sk == R.Meta.Chosen);
			if (st != null) Ui.Text(Body, Ui.Fit(R.CardName(st.Natures, st.Level), 26), 426, 216, new Color(R.CardColor(st.Natures)), width: 164, align: HorizontalAlignment.Center);
			else Ui.Text(Body, "NONE", 426, 216, Pal.Dim, width: 164, align: HorizontalAlignment.Center);
			Ui.Tile(Body, 594, 210, 22, 22, () => { R.StepChosen(1); Commit(); }, ">", Pal.Void);
		}

		bool ready = R.DeckCount() >= R.DeckMin();
		Ui.Tile(Body, 198, 248, 220, 34, () => Say(R.BeginAdventure(), Pal.Gold), "ENTER THE RIFT", Pal.Mint, ready, big: true);
		if (!ready) Ui.Text(Body, $"THE POOL NEEDS {R.DeckMin()}", 198, 286, Pal.Warn, width: 220, align: HorizontalAlignment.Center);
	}
}

public class TemplateCutscene : TaroCutsceneScript
{
	private new void Awake()
	{
		base.Awake();
		dialog.Add("`p,1;Here `s,.2;we are at my new `v;dialog engine text`n;. `s,.04;It's gonna make text `w;Wavey!`n;");
		dialog.Add("`c,0,1;I'm `v;<color=#FF0000>serious</color>`n;, like `w;seriously Wavey!`n;... And gotta test ellipsis...");
		dialog.Add("`p,.6;`c,0,0;...");
		dialog.Add("`p,1;Only if it's the first letter, huh?");
		dialog.Add("Testing `c,0,1;`p,.6;Pauses in speech...");
	}

	private void Start()
	{
		ADOBase.controller.isCutscene = true;
	}

	private new void Update()
	{
		base.Update();
	}
}

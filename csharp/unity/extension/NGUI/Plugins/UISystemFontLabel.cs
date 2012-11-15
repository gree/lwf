using UnityEngine;

using Style = LWF.UnityRenderer.ISystemFontRenderer.Style;
using Align = LWF.UnityRenderer.ISystemFontRenderer.Align;
using VerticalAlign = LWF.UnityRenderer.ISystemFontRenderer.VerticalAlign;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/SystemFontLabel")]
public class UISystemFontLabel : UIWidget
{
	[SerializeField]
	protected SystemFont.Renderer mRenderer = new SystemFont.Renderer();
	[SerializeField] protected bool mPropertyChanged;

	[SerializeField] protected float mSize;
	[SerializeField] protected float mWidth;
	[SerializeField] protected float mHeight;
	[SerializeField] protected Style mStyle;
	[SerializeField] protected Align mAlign;
	[SerializeField] protected VerticalAlign mVerticalAlign;
	[SerializeField] protected float mLineSpacing;
	[SerializeField] protected float mLetterSpacing;
	[SerializeField] protected float mLeftMargin;
	[SerializeField] protected float mRightMargin;
	[SerializeField] protected string mText;

	public float size {
		get {return mSize;}
		set {mSize = value; mPropertyChanged = true;}
	}
	public float width {
		get {return mWidth;}
		set {mWidth = value; mPropertyChanged = true;}
	}
	public float height {
		get {return mHeight;}
		set {mHeight = value; mPropertyChanged = true;}
	}
	public Style style {
		get {return mStyle;}
		set {mStyle = value; mPropertyChanged = true;}
	}
	public Align align {
		get {return mAlign;}
		set {mAlign = value; mPropertyChanged = true;}
	}
	public VerticalAlign verticalAlign {
		get {return mVerticalAlign;}
		set {mVerticalAlign = value; mPropertyChanged = true;}
	}
	public float lineSpacing {
		get {return mLineSpacing;}
		set {mLineSpacing = value; mPropertyChanged = true;}
	}
	public float letterSpacing {
		get {return mLetterSpacing;}
		set {mLetterSpacing = value; mPropertyChanged = true;}
	}
	public float leftMargin {
		get {return mLeftMargin;}
		set {mLeftMargin = value; mPropertyChanged = true;}
	}
	public float rightMargin {
		get {return mRightMargin;}
		set {mRightMargin = value; mPropertyChanged = true;}
	}
	public string text {
		get {return mText;}
		set {mText = value;}
	}

	public override bool keepMaterial {get {return true;}}

	override protected void OnStart()
	{
		mRenderer.Init(mSize, mWidth, mHeight,
			mStyle, mAlign, mVerticalAlign,
			mLineSpacing, mLetterSpacing, mLeftMargin, mRightMargin);
		mRenderer.SetText(mText, color);

		material = mRenderer.material;
	}

	override public void OnFill(BetterList<Vector3> verts,
		BetterList<Vector2> uvs, BetterList<Color> cols)
	{
		Vector3[] v = mRenderer.mesh.vertices;
		verts.Add(v[0]);
		verts.Add(v[1]);
		verts.Add(v[3]);
		verts.Add(v[2]);

		Vector2[] u = mRenderer.mesh.uv;
		uvs.Add(u[0]);
		uvs.Add(u[1]);
		uvs.Add(u[3]);
		uvs.Add(u[2]);

		Color c = mRenderer.color;
		cols.Add(c);
		cols.Add(c);
		cols.Add(c);
		cols.Add(c);

		material = mRenderer.material;
	}

	public override bool OnUpdate()
	{
		bool changed = false;

		if (mPropertyChanged) {
			mPropertyChanged = false;
			mRenderer.Init(
				mSize, mWidth, mHeight, mStyle, mAlign, mVerticalAlign,
				mLineSpacing, mLetterSpacing, mLeftMargin, mRightMargin);
			changed = true;
		} else if (!mRenderer.GetText().Equals(mText) || mChanged) {
			changed = true;
		}
		if (changed)
			mRenderer.SetText(mText, color);

		return true;
	}

	void OnDestroy()
	{
		material = null;
		mRenderer.Destruct();
	}
}

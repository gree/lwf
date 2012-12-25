using UnityEngine;

using Align = BitmapFont.Renderer.Align;
using VerticalAlign = BitmapFont.Renderer.VerticalAlign;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/BitmapFontLabel")]
public class UIBitmapFontLabel : UIWidget
{
	[SerializeField]
	protected BitmapFont.Renderer mRenderer = new BitmapFont.Renderer();
	[SerializeField] protected bool mPropertyChanged;

	[SerializeField] protected string mFontName;
	[SerializeField] protected float mSize;
	[SerializeField] protected float mWidth;
	[SerializeField] protected float mHeight;
	[SerializeField] protected Align mAlign;
	[SerializeField] protected VerticalAlign mVerticalAlign;
	[SerializeField] protected float mSpaceAdvance;
	[SerializeField] protected float mLineSpacing;
	[SerializeField] protected float mLetterSpacing;
	[SerializeField] protected float mTabSpacing;
	[SerializeField] protected float mLeftMargin;
	[SerializeField] protected float mRightMargin;
	[SerializeField] protected string mText;

	public string fontName {
		get {return mFontName;}
		set {mFontName = value; mPropertyChanged = true;}
	}
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
	public Align align {
		get {return mAlign;}
		set {mAlign = value; mPropertyChanged = true;}
	}
	public VerticalAlign verticalAlign {
		get {return mVerticalAlign;}
		set {mVerticalAlign = value; mPropertyChanged = true;}
	}
	public float spaceAdvance {
		get {return mSpaceAdvance;}
		set {mSpaceAdvance = value; mPropertyChanged = true;}
	}
	public float lineSpacing {
		get {return mLineSpacing;}
		set {mLineSpacing = value; mPropertyChanged = true;}
	}
	public float letterSpacing {
		get {return mLetterSpacing;}
		set {mLetterSpacing = value; mPropertyChanged = true;}
	}
	public float tabSpacing {
		get {return mTabSpacing;}
		set {mTabSpacing = value; mPropertyChanged = true;}
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
		if (string.IsNullOrEmpty(mFontName)) {
			Debug.LogWarning(
				"UIBitmapFontLabel: fontName should be a correct fontName");
			return;
		}

		mRenderer.Init(mFontName, mSize, mWidth, mHeight, mAlign,
			mVerticalAlign, mSpaceAdvance, mLineSpacing, mLetterSpacing,
			mTabSpacing, mLeftMargin, mRightMargin);
		mRenderer.SetText(mText, color);

		material = mRenderer.material;
	}

	override public void OnFill(BetterList<Vector3> verts,
		BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector3[] v = mRenderer.mesh.vertices;
		Vector2[] u = mRenderer.mesh.uv;
		Color c = color;
		int n = v.Length;

		for (int i = 0; i < n; i += 4) {
			verts.Add(v[i + 0]);
			verts.Add(v[i + 1]);
			verts.Add(v[i + 3]);
			verts.Add(v[i + 2]);

			uvs.Add(u[i + 0]);
			uvs.Add(u[i + 1]);
			uvs.Add(u[i + 3]);
			uvs.Add(u[i + 2]);

			cols.Add(c);
			cols.Add(c);
			cols.Add(c);
			cols.Add(c);
		}

		material = mRenderer.material;
	}

	public override bool OnUpdate()
	{
		bool changed = false;

		if (mPropertyChanged) {
			mPropertyChanged = false;

			if (string.IsNullOrEmpty(mFontName)) {
				Debug.LogWarning(
					"UIBitmapFontLabel: fontName should be a correct fontName");
				return false;
			}

			mRenderer.Init(mFontName, mSize, mWidth, mHeight, mAlign,
				mVerticalAlign, mSpaceAdvance, mLineSpacing, mLetterSpacing,
				mTabSpacing, mLeftMargin, mRightMargin);
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

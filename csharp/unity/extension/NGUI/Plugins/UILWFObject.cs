using UnityEngine;

[AddComponentMenu("NGUI/UI/LWFObject")]
public class UILWFObject : UIWidget
{
	public enum ScaleType {
		NORMAL,
		FIT_FOR_HEIGHT,
		FIT_FOR_WIDTH,
		SCALE_FOR_HEIGHT,
		SCALE_FOR_WIDTH,
	}

	[SerializeField] protected LWFObject mLWFObject;
	[SerializeField] protected bool mPropertyChanged;

	[SerializeField] protected string mPath;
	[SerializeField] protected float mZOffset;
	[SerializeField] protected float mZRate;
	[SerializeField] protected int mRenderQueueOffset;
	[SerializeField] protected ScaleType mScaleType;

	public string path {
		get {return mPath;}
		set {mPath = value; mPropertyChanged = true;}
	}
	public float zOffset {
		get {return mZOffset;}
		set {mZOffset = value; mPropertyChanged = true;}
	}
	public float zRate {
		get {return mZRate;}
		set {mZRate = value; mPropertyChanged = true;}
	}
	public int renderQueueOffset {
		get {return mRenderQueueOffset;}
		set {mRenderQueueOffset = value; mPropertyChanged = true;}
	}
	public ScaleType scaleType {
		get {return mScaleType;}
		set {mScaleType = value; mPropertyChanged = true;}
	}
	public LWFObject lwfObject {
		get {return mLWFObject;}
	}

	void DestroyLWF()
	{
		if (mLWFObject != null) {
			if (Application.isPlaying)
				Destroy(mLWFObject.gameObject);
			else
				DestroyImmediate(mLWFObject.gameObject);
			mLWFObject = null;
		}
	}

	void InitLWF()
	{
		DestroyLWF();

		if (string.IsNullOrEmpty(mPath)) {
			Debug.LogWarning(
				"UILWFObject: path should be a correct lwf bytes path");
			return;
		}

		string texturePrefix = System.IO.Path.GetDirectoryName(mPath);
		if (texturePrefix.Length > 0)
			texturePrefix += "/";

		GameObject o = new GameObject(mPath);
		o.layer = gameObject.layer;
		o.hideFlags = HideFlags.HideAndDontSave;

		Transform transform = o.transform;
		transform.parent = gameObject.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;

		Camera camera = NGUITools.FindCameraForLayer(o.layer);
		mLWFObject = o.AddComponent<LWFObject>();
		mLWFObject.Load(mPath, texturePrefix, "",
			mZOffset, mZRate, mRenderQueueOffset, camera, true);

		int height = (int)camera.orthographicSize * 2;
		int width = (int)(camera.aspect * (float)height);
		switch (mScaleType) {
		case ScaleType.FIT_FOR_HEIGHT:
			mLWFObject.FitForHeight(height);
			break;
		case ScaleType.FIT_FOR_WIDTH:
			mLWFObject.FitForWidth(width);
			break;
		case ScaleType.SCALE_FOR_HEIGHT:
			mLWFObject.ScaleForHeight(height);
			break;
		case ScaleType.SCALE_FOR_WIDTH:
			mLWFObject.ScaleForWidth(width);
			break;
		}
	}

	override protected void OnStart()
	{
		InitLWF();
	}

	public override bool OnUpdate()
	{
		if (mPropertyChanged) {
			mPropertyChanged = false;
			InitLWF();
			return true;
		}

		return false;
	}

	void OnDestroy()
	{
		DestroyLWF();
	}
}

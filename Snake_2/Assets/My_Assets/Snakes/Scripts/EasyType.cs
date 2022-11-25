using System;

public class EasyType
{
	public static float Liner(float t)
	{
		return t;
	}

	public static float EaseInQuad(float t)
	{
		return t * t;
	}

	public static float EaseOutQuad(float t)
	{
		return t * (2f - t);
	}

	public static float EaseInOutQuad(float t)
	{
		if (!(t < 0.5f))
		{
			return -1f + (4f - 2f * t) * t;
		}
		return 2f * t * t;
	}

	public static float EaseInCubic(float t)
	{
		return t * t * t;
	}

	public static float EaseOutCubic(float t)
	{
		return (t -= 1f) * t * t + 1f;
	}

	public static float EaseInOutCubic(float t)
	{
		if (!(t < 0.5f))
		{
			return (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f;
		}
		return 4f * t * t * t;
	}

	public static float EaseInQuart(float t)
	{
		return t * t * t * t;
	}

	public static float EaseOutQuart(float t)
	{
		return 1f - (t -= 1f) * t * t * t;
	}

	public static float EaseInOutQuart(float t)
	{
		if (!(t < 0.5f))
		{
			return 1f - 8f * (t -= 1f) * t * t * t;
		}
		return 8f * t * t * t * t;
	}

	public static float EaseInQuint(float t)
	{
		return t * t * t * t * t;
	}

	public static float EaseOutQuint(float t)
	{
		return 1f + (t -= 1f) * t * t * t * t;
	}

	public static float EaseInOutQuint(float t)
	{
		if (!(t < 0.5f))
		{
			return 1f + 16f * (t -= 1f) * t * t * t * t;
		}
		return 16f * t * t * t * t * t;
	}

	public static float EaseInElastic(float t)
	{
		return (float)((double)(0.04f - 0.04f / t) * Math.Sin(25f * t) + 1.0);
	}

	public static float EaseOutElastic(float t)
	{
		return (float)((double)(0.04f * t / (t -= 1f)) * Math.Sin(25f * t));
	}

	public static float EaseInOutElastic(float t)
	{
		return (float)(((t -= 0.5f) < 0f) ? ((double)(0.02f + 0.01f / t) * Math.Sin(50f * t)) : ((double)(0.02f - 0.01f / t) * Math.Sin(50f * t) + 1.0));
	}

	public static float MatchedLerpType(LerpType lerpType, float t)
	{
		switch (lerpType)
		{
		case LerpType.Liner:
			return Liner(t);
		case LerpType.EaseInQuad:
			return EaseInQuad(t);
		case LerpType.EaseOutQuad:
			return EaseOutQuad(t);
		case LerpType.EaseInOutQuad:
			return EaseInOutQuad(t);
		case LerpType.EaseInCubic:
			return EaseInCubic(t);
		case LerpType.EaseOutCubic:
			return EaseOutCubic(t);
		case LerpType.EaseInOutCubic:
			return EaseInOutCubic(t);
		case LerpType.EaseInQuart:
			return EaseInQuart(t);
		case LerpType.EaseOutQuart:
			return EaseOutQuart(t);
		case LerpType.EaseInOutQuart:
			return EaseInOutQuart(t);
		case LerpType.EaseInQuint:
			return EaseInQuint(t);
		case LerpType.EaseOutQuint:
			return EaseOutQuint(t);
		case LerpType.EaseInOutQuint:
			return EaseInOutQuint(t);
		case LerpType.EaseInElastic:
			return EaseInElastic(t);
		case LerpType.EaseOutElastic:
			return EaseOutElastic(t);
		case LerpType.EaseInOutElastic:
			return EaseInOutElastic(t);
		default:
			return t;
		}
	}
}

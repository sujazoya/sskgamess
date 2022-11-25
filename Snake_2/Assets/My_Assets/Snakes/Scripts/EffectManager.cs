using CBGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	[SerializeField]
	private GameObject easyBlockExplodePrefab;

	[SerializeField]
	private GameObject normalBlockExplodePrefab;

	[SerializeField]
	private GameObject mediumBlockExplodePrefab;

	[SerializeField]
	private GameObject hardBlockExplodePrefab;

	[SerializeField]
	private GameObject evilBlockExplodePrefab;

	[SerializeField]
	private GameObject bodyPartExplodePrefab;

	[SerializeField]
	private GameObject coinCollectPrefab;

	[SerializeField]
	private GameObject shieldCollectPrefab;

	private List<ParticleSystem> listEasyBlockExplode = new List<ParticleSystem>();

	private List<ParticleSystem> listNormalBlockExplode = new List<ParticleSystem>();

	private List<ParticleSystem> listMediumBlockExplode = new List<ParticleSystem>();

	private List<ParticleSystem> listHardBlockExplode = new List<ParticleSystem>();

	private List<ParticleSystem> listEvilBlockExplode = new List<ParticleSystem>();

	private List<ParticleSystem> listBodyPartExplode = new List<ParticleSystem>();

	private List<ParticleSystem> listCoinCollectParticle = new List<ParticleSystem>();

	private List<ParticleSystem> listShieldCollectParticle = new List<ParticleSystem>();

	public static EffectManager Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			return;
		}
		UnityEngine.Object.DestroyImmediate(Instance.gameObject);
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private IEnumerator CRPlayParticle(ParticleSystem par)
	{
		par.Play();
		yield return new WaitForSeconds(par.main.startLifetimeMultiplier);
		par.gameObject.SetActive(value: false);
	}

	private ParticleSystem GetBlockExplode(BlockType type)
	{
		switch (type)
		{
		case BlockType.EASY:
		{
			foreach (ParticleSystem item in listEasyBlockExplode)
			{
				if (!item.gameObject.activeInHierarchy)
				{
					return item;
				}
			}
			ParticleSystem component2 = UnityEngine.Object.Instantiate(easyBlockExplodePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
			component2.gameObject.SetActive(value: false);
			listEasyBlockExplode.Add(component2);
			return component2;
		}
		case BlockType.NORMAL:
		{
			foreach (ParticleSystem item2 in listNormalBlockExplode)
			{
				if (!item2.gameObject.activeInHierarchy)
				{
					return item2;
				}
			}
			ParticleSystem component4 = UnityEngine.Object.Instantiate(normalBlockExplodePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
			component4.gameObject.SetActive(value: false);
			listNormalBlockExplode.Add(component4);
			return component4;
		}
		case BlockType.MEDIUM:
		{
			foreach (ParticleSystem item3 in listMediumBlockExplode)
			{
				if (!item3.gameObject.activeInHierarchy)
				{
					return item3;
				}
			}
			ParticleSystem component5 = UnityEngine.Object.Instantiate(mediumBlockExplodePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
			component5.gameObject.SetActive(value: false);
			listMediumBlockExplode.Add(component5);
			return component5;
		}
		case BlockType.HARD:
		{
			foreach (ParticleSystem item4 in listHardBlockExplode)
			{
				if (!item4.gameObject.activeInHierarchy)
				{
					return item4;
				}
			}
			ParticleSystem component3 = UnityEngine.Object.Instantiate(hardBlockExplodePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
			component3.gameObject.SetActive(value: false);
			listHardBlockExplode.Add(component3);
			return component3;
		}
		default:
		{
			foreach (ParticleSystem item5 in listEvilBlockExplode)
			{
				if (!item5.gameObject.activeInHierarchy)
				{
					return item5;
				}
			}
			ParticleSystem component = UnityEngine.Object.Instantiate(evilBlockExplodePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
			component.gameObject.SetActive(value: false);
			listEvilBlockExplode.Add(component);
			return component;
		}
		}
	}

	private ParticleSystem GetBodyPartExplode()
	{
		foreach (ParticleSystem item in listBodyPartExplode)
		{
			if (!item.gameObject.activeInHierarchy)
			{
				return item;
			}
		}
		ParticleSystem component = UnityEngine.Object.Instantiate(bodyPartExplodePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
		component.gameObject.SetActive(value: false);
		listBodyPartExplode.Add(component);
		return component;
	}

	private ParticleSystem GetCoinCollectEffect()
	{
		foreach (ParticleSystem item in listCoinCollectParticle)
		{
			if (!item.gameObject.activeInHierarchy)
			{
				return item;
			}
		}
		ParticleSystem component = UnityEngine.Object.Instantiate(coinCollectPrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
		component.gameObject.SetActive(value: false);
		listCoinCollectParticle.Add(component);
		return component;
	}

	private ParticleSystem GetShieldCollectEffect()
	{
		foreach (ParticleSystem item in listShieldCollectParticle)
		{
			if (!item.gameObject.activeInHierarchy)
			{
				return item;
			}
		}
		ParticleSystem component = UnityEngine.Object.Instantiate(shieldCollectPrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
		component.gameObject.SetActive(value: false);
		listShieldCollectParticle.Add(component);
		return component;
	}

	public void CreateBlockExplode(BlockType type, Vector3 pos)
	{
		ParticleSystem blockExplode = GetBlockExplode(type);
		blockExplode.transform.position = pos;
		blockExplode.transform.eulerAngles = new Vector3(270f, 0f, 0f);
		blockExplode.gameObject.SetActive(value: true);
		StartCoroutine(CRPlayParticle(blockExplode));
	}

	public void CreateBodyPartExplode(Vector3 pos)
	{
		ParticleSystem bodyPartExplode = GetBodyPartExplode();
		bodyPartExplode.transform.position = pos;
		bodyPartExplode.gameObject.SetActive(value: true);
		StartCoroutine(CRPlayParticle(bodyPartExplode));
	}

	public void CreateCoinCollectEffect(Vector3 pos)
	{
		ParticleSystem coinCollectEffect = GetCoinCollectEffect();
		coinCollectEffect.transform.position = pos;
		coinCollectEffect.gameObject.SetActive(value: true);
		StartCoroutine(CRPlayParticle(coinCollectEffect));
	}

	public void CreateShieldCollectEffect(Vector3 pos)
	{
		ParticleSystem shieldCollectEffect = GetShieldCollectEffect();
		shieldCollectEffect.transform.position = pos;
		shieldCollectEffect.gameObject.SetActive(value: true);
		StartCoroutine(CRPlayParticle(shieldCollectEffect));
	}
}

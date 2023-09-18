//========================================
//
// SHAPE FX
// (c) 2016 - Jean Moreno
// http://www.jeanmoreno.com/unity
//
//========================================

//#define SHFX_DEBUG

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShapeFX
{
	[CustomEditor(typeof(ShFX_Variants))]
	#if SHFX_DEBUG
	[CanEditMultipleObjects()]
	#endif
	public class ShFX_Variants_Editor : Editor
	{
		// Compatibility fixes

	#if UNITY_5_5_OR_NEWER
		const string strEmissionModuleRateScalar = "EmissionModule.rateOverTime.scalar";
		const string strGravityModifer = "InitialModule.gravityModifier.scalar";
	#else
		const string strEmissionModuleRateScalar = "EmissionModule.rate.scalar";
		const string strGravityModifer = "InitialModule.gravityModifier";
	#endif

		static bool IsUsingDistanceEmission(SerializedObject so)
		{
	#if UNITY_5_5_OR_NEWER
		return true;
	#else
		var prop = so.FindProperty("EmissionModule.m_Type");
		return (prop != null) ? (prop.intValue == 1) : false;
	#endif
		}

		//--------------------------------------------------------------------------------------------------------------------------------

		private ShFX_Variants shfx_variants { get { return target as ShFX_Variants; } }

		private ParticleSystem _particleSystem;
		private ParticleSystem particleSystem
		{
			get
			{
				if(_particleSystem == null && shfx_variants != null)
				{
					_particleSystem = shfx_variants.GetComponent<ParticleSystem>();
				}
				return _particleSystem;
			}
		}

		private ParticleSystem[] _allParticleSystem;
		private ParticleSystem[] allParticleSystem
		{
			get
			{
				if((_allParticleSystem == null || _allParticleSystem.Length == 0) && shfx_variants != null)
				{
					_allParticleSystem = shfx_variants.GetComponentsInChildren<ParticleSystem>(true);
				}
				return _allParticleSystem;
			}
		}

		private SerializedObject _psSerializedObject;
		private SerializedObject psSerializedObject
		{
			get
			{
				if(_psSerializedObject == null)
				{
					_psSerializedObject = new SerializedObject(particleSystem);
				}
				return _psSerializedObject;
			}
		}

		private ShFX_Variants.EffectDensity[] AllEffectDensities
		{
			get
			{
				if(shfx_variants.effectDensities == null)
				{
					shfx_variants.effectDensities = new ShFX_Variants.EffectDensity[allParticleSystem.Length];
					for(int i = 0; i < shfx_variants.effectDensities.Length; i++)
					{
						shfx_variants.effectDensities[i] = CreateEffectDensity(allParticleSystem[i]);
					}
				}
				else if(shfx_variants.effectDensities.Length != allParticleSystem.Length)
				{
					//See if there are existing ParticleSystem already defined, else create new EffectDensity
					var newEffectDensities = new ShFX_Variants.EffectDensity[allParticleSystem.Length];
					for(int i = 0; i < allParticleSystem.Length; i++)
					{
						ShFX_Variants.EffectDensity effectDensity = null;
						foreach(ShFX_Variants.EffectDensity existingEffectDensity in shfx_variants.effectDensities)
						{
							if(existingEffectDensity.particleSystem == allParticleSystem[i])
							{
								effectDensity = existingEffectDensity;
								break;
							}
						}

						if(effectDensity == null)
						{
							effectDensity = CreateEffectDensity(allParticleSystem[i]);
						}

						newEffectDensities[i] = effectDensity;
					}

					shfx_variants.effectDensities = newEffectDensities;
				}

				return shfx_variants.effectDensities;
			}
		}

		private ShFX_Variants.EffectDensity CreateEffectDensity(ParticleSystem particleSystem)
		{
			var effectDensity = new ShFX_Variants.EffectDensity();
			effectDensity.particleSystem = particleSystem;
			SerializedProperty emissionRateProp = new SerializedObject(effectDensity.particleSystem).FindProperty(strEmissionModuleRateScalar);
			effectDensity.densityCachedEmissionRate = emissionRateProp.floatValue;
			effectDensity.density = emissionRateProp.floatValue;
			return effectDensity;
		}

		//--------------------------------------------------------------------------------------------------------------------------------

		public class ParticleEffect
		{
			public enum Shape
			{
				Unknown,
				Square,
				SquareRound,
				Circle,
				Triangle,
				TriangleFlip,
				SquareHollow,
				CircleHollow,
				TriangleHollow,
				TriangleFlipHollow,
			}

			public enum Style
			{
				Unknown,
				Crisp,
				Blur,
				Glow
			}

			public enum BlendMode
			{
				Unknown,
				AlphaBlended,
				AlphaBlendedPremultiply,
				Additive,
				AdditiveSoft,
				Multiplicative
			}

			static private string[] _shapeMenuItems;
			static private string[] shapeMenuItems
			{
				get
				{
					if(_shapeMenuItems == null)
					{
						List<string> namesList = new List<string>(System.Enum.GetNames(typeof(Shape)));
						namesList.RemoveAt(0);	//remove 'Unknown'
						_shapeMenuItems = namesList.ToArray();

						//Add spaces for lisibility
						for(int i = 0; i < _shapeMenuItems.Length; i++)
						{
							_shapeMenuItems[i] = _shapeMenuItems[i].Replace("Round", " Round");
							_shapeMenuItems[i] = _shapeMenuItems[i].Replace("Hollow", " Hollow");
						}
					}

					return _shapeMenuItems;
				}
			}

			static private string[] _styleMenuItems;
			static private string[] styleMenuItems
			{
				get
				{
					if(_styleMenuItems == null)
					{
						List<string> namesList = new List<string>(System.Enum.GetNames(typeof(Style)));
						namesList.RemoveAt(0);	//remove 'Unknown'
						_styleMenuItems = namesList.ToArray();
					}
					return _styleMenuItems;
				}
			}

			static private string[] _blendModeMenuItems;
			static private string[] blendModeMenuItems
			{
				get
				{
					if(_blendModeMenuItems == null)
					{
						List<string> namesList = new List<string>(System.Enum.GetNames(typeof(BlendMode)));
						namesList.RemoveAt(0);	//remove 'Unknown'
						_blendModeMenuItems = namesList.ToArray();

						//Add spaces for lisibility
						for(int i = 0; i < _blendModeMenuItems.Length; i++)
						{
							_blendModeMenuItems[i] = _blendModeMenuItems[i].Replace("Blended", " Blended");
							_blendModeMenuItems[i] = _blendModeMenuItems[i].Replace("Premultiply", " Premultiply");
							_blendModeMenuItems[i] = _blendModeMenuItems[i].Replace("Soft", " Soft");
						}
					}
					return _blendModeMenuItems;
				}
			}


			//--------

			public Shape shape;
			public Style style;
			public BlendMode blendMode;
			public bool noSoftParticles;

			private ParticleSystem particle;
			private ParticleSystemRenderer renderer;
			private string name;

			private SerializedProperty p_startColor;
			private SerializedProperty p_colorOverLifetime;
			private SerializedProperty p_colorBySpeed;

			private class OriginalColors
			{
				private Color minColor;
				private Color maxColor;
				private Color[] minGradient;
				private Color[] maxGradient;

				public OriginalColors(SerializedProperty property)
				{
					SerializedProperty gradientProperty = property.FindPropertyRelative("minGradient");
					minGradient = new Color[8];
					for(int i = 0; i < 8; i++)
					{
						minGradient[i] = gradientProperty.FindPropertyRelative("key" + i).colorValue;
					}

					gradientProperty = property.FindPropertyRelative("maxGradient");
					maxGradient = new Color[8];
					for(int i = 0; i < 8; i++)
					{
						maxGradient[i] = gradientProperty.FindPropertyRelative("key" + i).colorValue;
					}

					minColor = property.FindPropertyRelative("minColor").colorValue;
					maxColor = property.FindPropertyRelative("maxColor").colorValue;
				}

				public void RestoreColors(SerializedProperty property)
				{
					SerializedProperty gradientProperty = property.FindPropertyRelative("minGradient");
					for(int i = 0; i < 8; i++)
					{
						gradientProperty.FindPropertyRelative("key" + i).colorValue = minGradient[i];
					}
					
					gradientProperty = property.FindPropertyRelative("maxGradient");
					for(int i = 0; i < 8; i++)
					{
						gradientProperty.FindPropertyRelative("key" + i).colorValue = maxGradient[i];
					}
					
					property.FindPropertyRelative("minColor").colorValue = minColor;
					property.FindPropertyRelative("maxColor").colorValue = maxColor;

					property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				}
			}
			private OriginalColors startColor_org;
			private OriginalColors colorOverLifetime_org;
			private OriginalColors colorBySpeed_org;

			private class OriginalColorsLight
			{
				public bool isGradient;
				public Color color;
				public Gradient gradient;

				public OriginalColorsLight(ShFX_EffectLight lightEffect)
				{
					this.isGradient = lightEffect.useColorGradient;
					this.gradient = new Gradient();
					this.gradient.SetKeys(lightEffect.colorGradient.colorKeys, lightEffect.colorGradient.alphaKeys);
					this.color = lightEffect.GetComponent<Light>().color;
				}
				
				public void RestoreColors(ShFX_EffectLight lightEffect)
				{
					lightEffect.colorGradient.SetKeys(this.gradient.colorKeys, this.gradient.alphaKeys);
					if(!isGradient)
						lightEffect.GetComponent<Light>().color = this.color;

					EditorUtility.SetDirty(lightEffect);
				}
			}
			private OriginalColorsLight lightEffectColor_org;

			public class EffectScales
			{
				public ParticleSystem particleSystem;
				public bool isRoot;
				public Vector3 transformScale;
				public Vector3 localPosition;
				public float startSize;
				public float gravityModifier;
				public float startSpeed;
				public float emissionRate;
				public float velocityX;
				public float velocityY;
				public float velocityZ;
				public float clampVelocityX;
				public float clampVelocityY;
				public float clampVelocityZ;
				public float clampVelocityMagnitude;
				public float forceX;
				public float forceY;
				public float forceZ;
				
				public ShFX_EffectLight effectLight;
				public float lightRange;
			}
			private EffectScales effectScales;

			//--------

			public ParticleEffect(ParticleSystem particleSystem, string name, bool isMobile)
			{
				this.name = name;

				if(particleSystem == null)
				{
					Debug.LogError("No ParticleSystem provided when creating ParticleEffect!\n");
					return;
				}

				this.particle = particleSystem;

				//ParticleSystemRenderer
				renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

				//Properties
				string matName = renderer.sharedMaterial != null ? renderer.sharedMaterial.name : "";

				if(matName.StartsWith("ShFX_MobileAtlas"))
				{
					GetShapesFromMobileMaterial();
				}
				else
				{
					shape = ParticleEffect.MaterialNameToShape(matName);
					style = ParticleEffect.MaterialNameToStyle(matName);
				}
				blendMode = ParticleEffect.MaterialNameToBlendMode(matName);
				noSoftParticles = matName.Contains("NoSP");

				//Fix desktop/mobile in case of undo/redo performed
				if(matName.StartsWith("ShFX_MobileAtlas") && !isMobile)
				{
					ConvertToDesktop();
				}
				else if(!matName.StartsWith("ShFX_MobileAtlas") && isMobile)
				{
					ConvertToMobile();
				}

				//Color Serialized Properties
				SerializedObject so = new SerializedObject(particleSystem);
				p_startColor = so.FindProperty("InitialModule.startColor");
				p_colorOverLifetime = so.FindProperty("ColorModule.gradient");
				p_colorBySpeed = so.FindProperty("ColorBySpeedModule.gradient");
			}

			private void GetShapesFromMobileMaterial()
			{
				SerializedObject so = new SerializedObject(this.particle);
				SerializedProperty prop_frameOverTime = so.FindProperty("UVModule.frameOverTime.scalar");
				SerializedProperty prop_tilesX = so.FindProperty("UVModule.tilesX");
				SerializedProperty prop_tilesY = so.FindProperty("UVModule.tilesY");
				if(prop_frameOverTime != null && prop_tilesX != null && prop_tilesY != null)
				{
					int size = prop_tilesX.intValue * prop_tilesY.intValue;
					int frameIndex = Mathf.CeilToInt(prop_frameOverTime.floatValue * size);
//					int frameIndex = shapeIndex*3 + styleIndex;
					this.shape = (Shape)((frameIndex/3)+1);
					this.style = (Style)((frameIndex % 3)+1);
				}
				else
				{
					Debug.LogError("ShapeFX - GetShapesFromMobileMaterial: Couldn't find 'UVModule' properties in ParticleSystem");
					this.shape = Shape.Unknown;
					this.style = Style.Unknown;
				}
			}

			public void UpdateSerializedObject()
			{
				if(p_startColor != null)
					p_startColor.serializedObject.Update();
			}

			public void UpdateMaterial(bool mobile = false, bool undo = true)
			{
				Material mat = GetMaterial(mobile);
				if(mat != null)
				{
					if(this.renderer.sharedMaterial != mat)
					{
						if(undo)
							Undo.RecordObject(this.renderer, "ShapeFX: update effect variant");
						this.renderer.sharedMaterial = mat;
						if(undo)
							EditorUtility.SetDirty(this.renderer);
					}
				}
			}

			public void ConvertToDesktop()
			{
				SerializedObject so = new SerializedObject(this.particle);
				SerializedProperty prop_enabled = so.FindProperty("UVModule.enabled");
				if(prop_enabled != null)
					prop_enabled.boolValue = false;
				else
					Debug.LogError("ShapeFX - ConvertToDesktop: Couldn't find 'UVModule.enabled' property in ParticleSystem");

				so.ApplyModifiedPropertiesWithoutUndo();
				UpdateMaterial(false, false);
			}

			public void ConvertToMobile()
			{
				int shapeIndex = (int)(this.shape-1);	//adjusted without Unknown type
				int styleIndex = (int)(this.style-1);	//adjusted without Unknown type
				int frameIndex = shapeIndex*3 + styleIndex;

				SerializedObject so = new SerializedObject(this.particle);
				SerializedProperty prop_enabled = so.FindProperty("UVModule.enabled");
				SerializedProperty prop_frameOverTime = so.FindProperty("UVModule.frameOverTime");
				SerializedProperty prop_tilesX = so.FindProperty("UVModule.tilesX");
				SerializedProperty prop_tilesY = so.FindProperty("UVModule.tilesY");
				SerializedProperty prop_animType = so.FindProperty("UVModule.animationType");

				//Constant values
				if(prop_enabled != null)
					prop_enabled.boolValue = true;
				else
					Debug.LogError("ShapeFX - ConvertToMobile: Couldn't find 'UVModule.enabled' property in ParticleSystem");

				if(prop_animType != null)
					prop_animType.intValue = 0;
				else
					Debug.LogError("ShapeFX - ConvertToMobile: Couldn't find 'UVModule.animationType' property in ParticleSystem");

				//Atlas values
				if(prop_tilesX != null)
					prop_tilesX.intValue = 6;
				else
					Debug.LogError("ShapeFX - ConvertToMobile: Couldn't find 'UVModule.tilesX' property in ParticleSystem");

				if(prop_tilesY != null)
					prop_tilesY.intValue = 5;
				else
					Debug.LogError("ShapeFX - ConvertToMobile: Couldn't find 'UVModule.tilesY' property in ParticleSystem");

				if(prop_frameOverTime != null)
				{
					prop_frameOverTime.FindPropertyRelative("minMaxState").intValue = 0;
					prop_frameOverTime.FindPropertyRelative("scalar").floatValue = (float)frameIndex/(prop_tilesX.intValue * prop_tilesY.intValue);
				}
				else
					Debug.LogError("ShapeFX - ConvertToMobile: Couldn't find 'UVModule.frameOverTime' property in ParticleSystem");

				so.ApplyModifiedPropertiesWithoutUndo();
				UpdateMaterial(true, false);
			}
			
			private Material GetMaterial(bool mobile)
			{
				if(mobile && !(this.shape == Shape.Square && this.style == Style.Crisp))
				{
					string dir = GetMobileMaterialsDirectory();
					if(dir == null)
						return null;
					string mobMaterialName = "ShFX_MobileAtlas_";
					if(this.blendMode == BlendMode.Unknown) return null;
					mobMaterialName += this.blendMode.ToString();
					if(this.noSoftParticles) mobMaterialName += " NoSP";

					string mobAssetPath = dir + mobMaterialName + ".mat";
					Material mobileMat = AssetDatabase.LoadAssetAtPath(mobAssetPath, typeof(Material)) as Material;
					if(mobileMat == null)
					{
						Debug.LogError("Can't find ShapeFX mobile atlas material");
						EditorApplication.Beep();
					}
					return mobileMat;
				}

				string materialName = "ShFX_";
				
				if(this.shape == Shape.Unknown) return null;
				materialName += this.shape.ToString() + "_";
				
				if(this.style == Style.Unknown) return null;
				materialName += this.style.ToString() + "_";
				
				if(this.blendMode == BlendMode.Unknown) return null;
				materialName += this.blendMode.ToString();
				
				if(this.noSoftParticles) materialName += " NoSP";
				
				string assetPath = GetMaterialsDirectory(this.noSoftParticles);
				if(assetPath == null)
					return null;

				assetPath = assetPath + materialName + ".mat";
				Material mat = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material)) as Material;
				if(mat == null)
				{
					Debug.LogError(string.Format("Can't find ShapeFX material for these properties:\n{0}, {1}, {2}{3} (\"{4}\")", this.shape, this.style, this.blendMode, noSoftParticles ? ", NoSP" : "", materialName));
					EditorApplication.Beep();
				}
				return mat;
			}

			public void Randomize(bool shape = true, bool style = true, bool blendMode = true)
			{
				if(shape)
				{
					Shape[] shapes = (Shape[])System.Enum.GetValues(typeof(Shape));
					this.shape = shapes[Random.Range(1, shapes.Length)];
				}
				if(style)
				{
					Style[] styles = (Style[])System.Enum.GetValues(typeof(Style));
					this.style = styles[Random.Range(1, styles.Length)];
				}
				if(blendMode)
				{
					BlendMode[] blendModes = (BlendMode[])System.Enum.GetValues(typeof(BlendMode));
					this.blendMode = blendModes[Random.Range(1, blendModes.Length)];
				}
			}

			//=============================================================================================================================================
			// COLOR EDITING
			public void GetOriginalColors()
			{
				UpdateSerializedObject();

				startColor_org = new OriginalColors(p_startColor);
				colorOverLifetime_org = new OriginalColors(p_colorOverLifetime);
				colorBySpeed_org = new OriginalColors(p_colorBySpeed);

				var lightEffect = GetLightEffect(this.particle);
				if(lightEffect != null)
				{
					lightEffectColor_org = new OriginalColorsLight(lightEffect);
				}
			}

			public void RevertColors()
			{
				if(startColor_org == null || colorOverLifetime_org == null || colorBySpeed_org == null)
				{
					Debug.LogError("Can't revert colors before getting the original colors!");
					EditorApplication.Beep();
					return;
				}

				startColor_org.RestoreColors(p_startColor);
				colorOverLifetime_org.RestoreColors(p_colorOverLifetime);
				colorBySpeed_org.RestoreColors(p_colorBySpeed);

				if(lightEffectColor_org != null)
				{
					var lightEffect = GetLightEffect(this.particle);
					if(lightEffect != null)
					{
						lightEffectColor_org.RestoreColors(lightEffect);
					}
				}

				startColor_org = null;
				colorOverLifetime_org = null;
				colorBySpeed_org = null;
				lightEffectColor_org = null;
			}

			public void InvertColors(bool affectLight = true)
			{
				GetOriginalColors();
				
				InvertColors(p_startColor);
				InvertColors(p_colorOverLifetime);
				InvertColors(p_colorBySpeed);

				p_startColor.serializedObject.ApplyModifiedProperties();
				p_colorOverLifetime.serializedObject.ApplyModifiedProperties();
				p_colorBySpeed.serializedObject.ApplyModifiedProperties();

				if(lightEffectColor_org != null && affectLight)
				{
					var lightEffect = GetLightEffect(this.particle);
					if(lightEffect != null)
					{
						InvertColorsLight(lightEffect);
					}
				}

				GetOriginalColors();
			}

			public void InvertColors(SerializedProperty colorsProperty)
			{
				InvertGradientProperty( colorsProperty.FindPropertyRelative("minGradient") );
				InvertGradientProperty( colorsProperty.FindPropertyRelative("maxGradient") );
				InvertColorProperty( colorsProperty.FindPropertyRelative("minColor") );
				InvertColorProperty( colorsProperty.FindPropertyRelative("maxColor") );
			}

			public void InvertColorsLight(ShFX_EffectLight lightEffect)
			{
				SerializedObject so = new SerializedObject(lightEffect);
				InvertGradientProperty( so.FindProperty("colorGradient") );
				so.ApplyModifiedPropertiesWithoutUndo();
				so.Dispose();
				
				SerializedObject lightSo = new SerializedObject(lightEffect.GetComponent<Light>());
				InvertColorProperty( lightSo.FindProperty("m_Color") );
				lightSo.ApplyModifiedPropertiesWithoutUndo();
				lightSo.Dispose();
			}

			public void AdjustColors(float hue, float saturation, float value, bool forceSaturation = false, bool affectLight = true)
			{
				ColorHSV hsv = new ColorHSV(hue, saturation, value);
				hsv.forceSaturation = forceSaturation;

				startColor_org.RestoreColors(p_startColor);
				colorOverLifetime_org.RestoreColors(p_colorOverLifetime);
				colorBySpeed_org.RestoreColors(p_colorBySpeed);

				AdjustColors(p_startColor, hsv);
				AdjustColors(p_colorOverLifetime, hsv);
				AdjustColors(p_colorBySpeed, hsv);

				p_startColor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				p_colorOverLifetime.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				p_colorBySpeed.serializedObject.ApplyModifiedPropertiesWithoutUndo();

				if(lightEffectColor_org != null && affectLight)
				{
					var lightEffect = GetLightEffect(this.particle);
					if(lightEffect != null)
					{
						lightEffectColor_org.RestoreColors(lightEffect);
						AdjustColorsLight(lightEffect, hsv);
					}
				}
			}

			static private void AdjustColors(SerializedProperty colorsProperty, ColorHSV hsv)
			{
				AdjustGradientProperty( colorsProperty.FindPropertyRelative("minGradient"), hsv );
				AdjustGradientProperty( colorsProperty.FindPropertyRelative("maxGradient"), hsv );
				AdjustColorProperty( colorsProperty.FindPropertyRelative("minColor"), hsv );
				AdjustColorProperty( colorsProperty.FindPropertyRelative("maxColor"), hsv );
			}

			static private void AdjustColorsLight(ShFX_EffectLight lightEffect, ColorHSV hsv)
			{
				SerializedObject so = new SerializedObject(lightEffect);
				AdjustGradientProperty( so.FindProperty("colorGradient"), hsv );
				so.ApplyModifiedPropertiesWithoutUndo();
				so.Dispose();

				SerializedObject lightSo = new SerializedObject(lightEffect.GetComponent<Light>());
				AdjustColorProperty( lightSo.FindProperty("m_Color"), hsv );
				lightSo.ApplyModifiedPropertiesWithoutUndo();
				lightSo.Dispose();
			}

			static private void AdjustGradientProperty(SerializedProperty gradientProperty, ColorHSV hsv)
			{
				int keys = gradientProperty.FindPropertyRelative("m_NumColorKeys").intValue;

				//Ignore if gradient is white only, as it probably is used to affect alpha value instead
				bool allWhite = true;
				for(int i = 0; i < keys; i++)
				{
					Color col = gradientProperty.FindPropertyRelative("key" + i).colorValue;
					col.a = 1f;
					if(col != Color.white)
					{
						allWhite = false;
						break;
					}
				}

				if(!allWhite)
				{
					for(int i = 0; i < keys; i++)
						AdjustColorProperty(gradientProperty.FindPropertyRelative("key" + i), hsv);
				}
			}

			static private void AdjustColorProperty(SerializedProperty colorProperty, ColorHSV hsv)
			{
				float alpha = colorProperty.colorValue.a;
				ColorHSV _hsv = RGBtoHSV(colorProperty.colorValue);
				bool ignoreSaturation = (colorProperty.colorValue.r == colorProperty.colorValue.g) && (colorProperty.colorValue.r == colorProperty.colorValue.b);
				if(hsv.h != 0)
				{
					//wrap hue value
					_hsv.h = _hsv.h + hsv.h;
					while(_hsv.h < 0)
						_hsv.h += 1f;
					while(_hsv.h > 1f)
						_hsv.h -= 1f;
				}
				if(hsv.s != 0 && (!ignoreSaturation || hsv.forceSaturation))
					_hsv.s = Mathf.Clamp01(_hsv.s + hsv.s);
				if(hsv.v != 0)
					_hsv.v = Mathf.Clamp01(_hsv.v + hsv.v);
				Color color = HSVtoRGB(_hsv);
				color.a = alpha;
				colorProperty.colorValue = color;
			}

			static private void InvertGradientProperty(SerializedProperty gradientProperty)
			{
				int keys = gradientProperty.FindPropertyRelative("m_NumColorKeys").intValue;
				for(int i = 0; i < keys; i++)
					InvertColorProperty(gradientProperty.FindPropertyRelative("key" + i));
			}

			static private void InvertColorProperty(SerializedProperty colorProperty)
			{
				Color color = colorProperty.colorValue;

				bool grayScale = (colorProperty.colorValue.r == colorProperty.colorValue.g) && (colorProperty.colorValue.r == colorProperty.colorValue.b);
				if(grayScale)
				{
					return;
				}

				color.r = 1f - color.r;
				color.g = 1f - color.g;
				color.b = 1f - color.b;
				colorProperty.colorValue = color;
			}

			//=============================================================================================================================================
			// SCALE EDITING

			public void GetScaleValues(bool isRoot)
			{
				this.effectScales = GetParticleSystemScaleValues(this.particle, isRoot);
				
				ShFX_EffectLight shfx_light = this.particle.GetComponentInChildren<ShFX_EffectLight>();
				if(shfx_light != null)
				{
					this.effectScales.effectLight = shfx_light;
					this.effectScales.lightRange = shfx_light.lightComponent.range;
				}
			}
			
			static private EffectScales GetParticleSystemScaleValues(ParticleSystem ps, bool isRoot)
			{
				EffectScales effectScales = new EffectScales();
				effectScales.particleSystem = ps;
				effectScales.isRoot = isRoot;
				effectScales.transformScale = ps.transform.localScale;
				effectScales.localPosition = isRoot ? Vector3.zero : ps.transform.localPosition;
				
				SerializedObject so = new SerializedObject(ps);
				effectScales.startSize = so.FindProperty("InitialModule.startSize.scalar").floatValue;
				effectScales.startSpeed = so.FindProperty("InitialModule.startSpeed.scalar").floatValue;
				effectScales.gravityModifier = so.FindProperty(strGravityModifer).floatValue;

				bool distanceEmission = IsUsingDistanceEmission(so);
				effectScales.emissionRate = distanceEmission ? so.FindProperty(strEmissionModuleRateScalar).floatValue : 0f;
				
				effectScales.velocityX = so.FindProperty("VelocityModule.x.scalar").floatValue;
				effectScales.velocityY = so.FindProperty("VelocityModule.y.scalar").floatValue;
				effectScales.velocityZ = so.FindProperty("VelocityModule.z.scalar").floatValue;
				
				effectScales.clampVelocityX = so.FindProperty("ClampVelocityModule.x.scalar").floatValue;
				effectScales.clampVelocityY = so.FindProperty("ClampVelocityModule.y.scalar").floatValue;
				effectScales.clampVelocityZ = so.FindProperty("ClampVelocityModule.z.scalar").floatValue;
				effectScales.clampVelocityMagnitude = so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue;
				
				effectScales.forceX = so.FindProperty("ForceModule.x.scalar").floatValue;
				effectScales.forceY = so.FindProperty("ForceModule.y.scalar").floatValue;
				effectScales.forceZ = so.FindProperty("ForceModule.z.scalar").floatValue;
				
				return effectScales;
			}
			
			public void SetScaleValues(float effectSize)
			{
				SetParticleSystemScaleValues(this.effectScales, effectSize);
			}
			private void SetParticleSystemScaleValues(EffectScales effectScales, float scale)
			{
				SerializedObject transformSo = new SerializedObject(effectScales.particleSystem.transform);
				if(effectScales.isRoot)
				{
					transformSo.FindProperty("m_LocalScale").vector3Value = effectScales.transformScale * scale;
				}
				
				SerializedObject so = new SerializedObject(effectScales.particleSystem);
				so.FindProperty("InitialModule.startSize.scalar").floatValue = effectScales.startSize * scale;
				if(effectScales.startSpeed > 0.01f || effectScales.startSpeed < -0.01f)
					so.FindProperty("InitialModule.startSpeed.scalar").floatValue = effectScales.startSpeed * scale;
				so.FindProperty(strGravityModifer).floatValue = effectScales.gravityModifier * scale;

				bool distanceEmission = IsUsingDistanceEmission(so);
				if(distanceEmission)
				{
					so.FindProperty(strEmissionModuleRateScalar).floatValue = effectScales.emissionRate * scale;
				}
				
				so.FindProperty("VelocityModule.x.scalar").floatValue = effectScales.velocityX * scale;
				so.FindProperty("VelocityModule.y.scalar").floatValue = effectScales.velocityY * scale;
				so.FindProperty("VelocityModule.z.scalar").floatValue = effectScales.velocityZ * scale;
				
				so.FindProperty("ClampVelocityModule.x.scalar").floatValue = effectScales.clampVelocityX * scale;
				so.FindProperty("ClampVelocityModule.y.scalar").floatValue = effectScales.clampVelocityY * scale;
				so.FindProperty("ClampVelocityModule.z.scalar").floatValue = effectScales.clampVelocityZ * scale;
				so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue = effectScales.clampVelocityMagnitude * scale;
				
				so.FindProperty("ForceModule.x.scalar").floatValue = effectScales.forceX * scale;
				so.FindProperty("ForceModule.y.scalar").floatValue = effectScales.forceY * scale;
				so.FindProperty("ForceModule.z.scalar").floatValue = effectScales.forceZ * scale;
				
				transformSo.ApplyModifiedPropertiesWithoutUndo();
				so.ApplyModifiedPropertiesWithoutUndo();
				
				if(effectScales.effectLight != null)
				{
					SerializedObject lightSO = new SerializedObject(effectScales.effectLight.lightComponent);
					lightSO.FindProperty("m_Range").floatValue = effectScales.lightRange * scale;
					lightSO.ApplyModifiedPropertiesWithoutUndo();
				}
			}

			//--------

			static public bool DrawGUI(ParticleEffect particleEffect)
			{
				return DrawGUI(particleEffect.name, ref particleEffect.shape, ref particleEffect.style, ref particleEffect.blendMode, ref particleEffect.noSoftParticles, EditorStyles.label);
			}
			static public bool DrawGUI(string label, ref Shape shape, ref Style style, ref BlendMode blendMode, ref bool noSoftParticles)
			{
				return DrawGUI(label, ref shape, ref style, ref blendMode, ref noSoftParticles, EditorStyles.label);
			}
			static public bool DrawGUI(string label, ref Shape shape, ref Style style, ref BlendMode blendMode, ref bool noSoftParticles, GUIStyle labelStyle, string[] overrideLabels = null)
			{
				bool changed = false;
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);

				GUILayout.Label(label, labelStyle, GUILayout.Width(TBL_LABEL_WIDTH));
				GUILayout.FlexibleSpace();

				//SHAPE
				int index = ((int)shape)-1;
				int selected = EditorGUILayout.Popup(index, shapeMenuItems, GUILayout.MaxWidth(TBL_SHAPE_MAXWIDTH), GUILayout.MinWidth(TBL_SHAPE_MINWIDTH));
				if(selected != index)
				{
					shape = (Shape)(selected+1);
				}
				Rect r = GUILayoutUtility.GetLastRect();
				if(overrideLabels != null && overrideLabels.Length == 3 && Event.current.type == EventType.Repaint)
				{
					EditorStyles.popup.Draw(r, new GUIContent(overrideLabels[0]), -1);
				}
				if(Event.current.type == EventType.MouseDown && Event.current.button == 1 && r.Contains(Event.current.mousePosition))
				{
					shape = GetEnumNextValue<Shape>(shape);
					if(shape == Shape.Unknown) shape = GetEnumNextValue<Shape>(shape);
					changed = true;
					Event.current.Use();
				}

				//STYLE
				index = ((int)style)-1;
				selected = EditorGUILayout.Popup(index, styleMenuItems, GUILayout.MaxWidth(TBL_STYLE_MAXWIDTH), GUILayout.MinWidth(TBL_STYLE_MINWIDTH));
				if(selected != index)
				{
					style = (Style)(selected+1);
				}
				r = GUILayoutUtility.GetLastRect();
				if(overrideLabels != null && overrideLabels.Length == 3 && Event.current.type == EventType.Repaint)
				{
					EditorStyles.popup.Draw(r, new GUIContent(overrideLabels[1]), -1);
				}
				if(Event.current.type == EventType.MouseDown && Event.current.button == 1 && r.Contains(Event.current.mousePosition))
				{
					style = GetEnumNextValue<Style>(style);
					if(style == Style.Unknown) style = GetEnumNextValue<Style>(style);
					changed = true;
					Event.current.Use();
				}

				//BLEND MODE
				index = ((int)blendMode)-1;
				selected = EditorGUILayout.Popup(index, blendModeMenuItems, GUILayout.MaxWidth(TBL_BLENDMODE_MAXWIDTH), GUILayout.MinWidth(TBL_BLENDMODE_MINWIDTH));
				if(selected != index)
				{
					blendMode = (BlendMode)(selected+1);
				}
				r = GUILayoutUtility.GetLastRect();
				if(overrideLabels != null && overrideLabels.Length == 3 && Event.current.type == EventType.Repaint)
				{
					EditorStyles.popup.Draw(r, new GUIContent(overrideLabels[2]), -1);
				}
				if(Event.current.type == EventType.MouseDown && Event.current.button == 1 && r.Contains(Event.current.mousePosition))
				{
					blendMode = GetEnumNextValue<BlendMode>(blendMode);
					if(blendMode == BlendMode.Unknown) blendMode = GetEnumNextValue<BlendMode>(blendMode);
					changed = true;
					Event.current.Use();
				}

				//SOFT PARTICLES
				bool softParticles = EditorGUILayout.Toggle(!noSoftParticles, GUILayout.Width(24f));
				if(softParticles == noSoftParticles)
				{
					noSoftParticles = !softParticles;
					changed = true;
				}

				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				if(EditorGUI.EndChangeCheck())
				{
					changed = true;
				}
				
				return changed;
			}

			static private string GetMobileMaterialsDirectory()
			{
				string root = "Assets/JMO Assets/Cube FX/Assets/Mobile/";

				Object obj = AssetDatabase.LoadAssetAtPath<Texture>(root + "ShFX_Mobile_Texture_Atlas.png");
				if(obj != null)
					return root;
				
				string[] files = System.IO.Directory.GetFiles(Application.dataPath, "ShFX_Mobile_Texture_Atlas.png", System.IO.SearchOption.AllDirectories);
				if(files == null || files.Length == 0)
				{
					Debug.LogError("Can't find ShapeFX texture atlas!\n(the file should be named 'ShFX_Mobile_Texture_Atlas.png')");
					EditorApplication.Beep();
					return null;
				}
				
				files[0] = files[0].Replace(Application.dataPath, "Assets");
				files[0] = files[0].Replace(@"\", "/");
				root = files[0].Substring(0, files[0].LastIndexOf("/")+1);

				return root;
			}

			static private string GetMaterialsDirectory(bool noSoftParticles)
			{
				string root = "Assets/JMO Assets/Cube FX/Assets/Materials Generated/";
				root += noSoftParticles ? "No Soft Particles/" : "Soft Particles/";

				Object obj = AssetDatabase.LoadAssetAtPath<Material>(root + "ShFX_Square_Crisp_AlphaBlended.mat");
				if(obj != null)
					return root;

				string[] files = System.IO.Directory.GetFiles(Application.dataPath, "ShFX_Square_Crisp_AlphaBlended.mat", System.IO.SearchOption.AllDirectories);
				if(files == null || files.Length == 0)
				{
					Debug.LogError("Can't find root path for ShapeFX materials!\n");
					EditorApplication.Beep();
					return null;
				}

				files[0] = files[0].Replace(Application.dataPath, "Assets");
				files[0] = files[0].Replace(@"\", "/");
				root = files[0].Substring(0, files[0].LastIndexOf("/")+1);
				if(noSoftParticles)
					root = root.Replace("Soft Particles", "No Soft Particles");

				return root;
			}

			static private T GetEnumNextValue<T>(T enumValue) where T : struct, System.IConvertible
			{
				int intValue = System.Convert.ToInt32(enumValue);
				intValue++;
				T[] values = (T[])System.Enum.GetValues(typeof(T));
				if(intValue >= values.Length) intValue = 0;
				return values[intValue];
			}

			static private Shape MaterialNameToShape(string name)
			{
				string[] names = System.Enum.GetNames(typeof(Shape));
				//sort and reverse to make sure that long names come first (e.g. "SquareRound" comes before "Square")
				System.Array.Sort<string>(names);
				System.Array.Reverse(names);

				for(int i = 0; i < names.Length; i++)
				{
					if(name.Contains(names[i]))
					{
						return (Shape)System.Enum.Parse(typeof(Shape), names[i]);
					}
				}

				return Shape.Unknown;
			}

			static private Style MaterialNameToStyle(string name)
			{
				string[] names = System.Enum.GetNames(typeof(Style));
				System.Array.Sort<string>(names);
				System.Array.Reverse(names);
				
				for(int i = 0; i < names.Length; i++)
				{
					if(name.Contains(names[i]))
					{
						return (Style)System.Enum.Parse(typeof(Style), names[i]);
					}
				}
				
				return Style.Unknown;
			}

			static private BlendMode MaterialNameToBlendMode(string name)
			{
				string[] names = System.Enum.GetNames(typeof(BlendMode));
				System.Array.Sort<string>(names);
				System.Array.Reverse(names);
				
				for(int i = 0; i < names.Length; i++)
				{
					if(name.Contains(names[i]))
					{
						return (BlendMode)System.Enum.Parse(typeof(BlendMode), names[i]);
					}
				}
				
				return BlendMode.Unknown;
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------

#if !SHFX_DEBUG
		PrefabType prefabType = PrefabType.None;
#endif
		ParticleEffect[] particleEffects;
		ShFX_EffectLight lightEffect;

		//Settings
		bool shapesFoldout = true;
		bool colorsFoldout = true;
		bool propsFoldout = true;
		bool lightFoldout = true;
		bool forceSaturation = false;
		bool affectLight = true;
		bool colorsPreviewParticles = false;

		//Edit All
		ParticleEffect.Shape allShape;
		ParticleEffect.Style allStyle;
		ParticleEffect.BlendMode allBlendMode;
		bool allSP;

		//Colors
		bool editingColors;
		float hueShift;
		float saturationShift;
		float valueShift;

		//Properties
		bool shouldUpdateEmission;
		bool editingScale;
		float scaleValue = 1f;

		//Light Preview
		float lastParticleEditorTime;
		
		//--------------------------------------------------------------------------------------------------------------------------------
		// Callbacks
		
		void OnEnable()
		{
#if !SHFX_DEBUG
			prefabType = PrefabUtility.GetPrefabType(this.target);
#endif
			GetEffects();
			StopEditingColors(true);
			StopEditingScale(true);

			LoadPrefs();
		}

		void OnDisable()
		{
			//Object has been destroyed
			if(this.target == null)
			{
				StopEditingColors();
				return;
			}

			if(editingColors)
			{
				StopEditingColorsDialog();
			}

			if(editingScale)
			{
				StopEditingScaleDialog();
			}

			SavePrefs();
		}

		void SavePrefs()
		{
			EditorPrefs.SetBool("ShapeFX_Variants shapesFoldout", shapesFoldout);
			EditorPrefs.SetBool("ShapeFX_Variants colorsFoldout", colorsFoldout);
			EditorPrefs.SetBool("ShapeFX_Variants propsFoldout", propsFoldout);
			EditorPrefs.SetBool("ShapeFX_Variants forceSaturation", forceSaturation);
			EditorPrefs.SetBool("ShapeFX_Variants affectLight", affectLight);
			EditorPrefs.SetBool("ShapeFX_Variants colorsPreviewParticles", colorsPreviewParticles);
			EditorPrefs.SetBool("ShapeFX_Variants lightFoldout", lightFoldout);
			EditorPrefs.SetBool("ShapeFX_Variants allSP", allSP);
		}

		void LoadPrefs()
		{
			shapesFoldout = EditorPrefs.GetBool("ShapeFX_Variants shapesFoldout", true);
			colorsFoldout = EditorPrefs.GetBool("ShapeFX_Variants colorsFoldout", true);
			propsFoldout = EditorPrefs.GetBool("ShapeFX_Variants propsFoldout", true);
			forceSaturation = EditorPrefs.GetBool("ShapeFX_Variants forceSaturation", false);
			affectLight = EditorPrefs.GetBool("ShapeFX_Variants affectLight", true);
			colorsPreviewParticles = EditorPrefs.GetBool("ShapeFX_Variants colorsPreviewParticles", true);
			lightFoldout = EditorPrefs.GetBool("ShapeFX_Variants lightFoldout", true);
			allSP = EditorPrefs.GetBool("ShapeFX_Variants allSP", true);
		}

		//--------------------------------------------------------------------------------------------------------------------------------
		// GUI

		const float TBL_LABEL_WIDTH = 130f;
		const float TBL_SHAPE_MINWIDTH = 0f;
		const float TBL_SHAPE_MAXWIDTH = 100f;
		const float TBL_STYLE_MINWIDTH = 0f;
		const float TBL_STYLE_MAXWIDTH = 70f;
		const float TBL_BLENDMODE_MINWIDTH = 0f;
		const float TBL_BLENDMODE_MAXWIDTH = 160f;

		static private ShFX_EffectLight GetLightEffect(Component comp)
		{
			ShFX_EffectLight[] lfx = comp.GetComponentsInChildren<ShFX_EffectLight>(true);
			return (lfx != null && lfx.Length > 0) ? lfx[0] : null;
		}

		public override void OnInspectorGUI()
		{
			//Update inspected Objects
#if UNITY_5_6_OR_NEWER
			psSerializedObject.UpdateIfRequiredOrScript();
#else
			psSerializedObject.UpdateIfDirtyOrScript();
#endif
			lightEffect = GetLightEffect(shfx_variants);

#if SHFX_DEBUG
			if(serializedObject.isEditingMultipleObjects)
			{
				goto DEBUG_PANEL;
			}
#endif

			//Handle density
			if(shfx_variants.useDensity)
			{
				bool emissionChanged;
				bool hasUpdated = false;
				foreach(ShFX_Variants.EffectDensity effectDensity in AllEffectDensities)
				{
					if(effectDensity.useDensity && ShouldUpdateEmission(effectDensity, out emissionChanged))
					{
						UpdateEmissionDensity(effectDensity, emissionChanged);
						hasUpdated = true;
					}
				}

				if(hasUpdated)
				{
					//update cached values
					shfx_variants.densityCachedScale = shfx_variants.transform.localScale;
					float scaleVolume = shfx_variants.transform.localScale.x * shfx_variants.transform.localScale.y * shfx_variants.transform.localScale.z;
					shfx_variants.densityCachedScaleVolume = scaleVolume;
					shouldUpdateEmission = false;
				}
			}

#if !SHFX_DEBUG
			if(prefabType == PrefabType.Prefab)
			{
				EditorGUILayout.HelpBox("This tool cannot be used on Prefabs to prevent data loss.\nPlease drop an instance of the prefab into your Scene, do your modifications on it, and then manually Apply on the Prefab or create a new Prefab with your settings.", MessageType.Warning);
				return;
			}
#endif

			EditorGUILayout.HelpBox("Use this editor to easily change the style and appearance of the effect.\n\nThis component is only a tool and will only be an empty component when you build your application.", MessageType.Info);
			Rect r = GUILayoutUtility.GetLastRect();
			if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && r.Contains(Event.current.mousePosition))
			{
				PreviewParticleSystem();
				Event.current.Use();
			}

			// PREVIEW

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("PLAY EFFECT", "ButtonLeft", GUILayout.Height(30f)))
			{
				PreviewParticleSystem();
			}
			if(GUILayout.Button("STOP EFFECT", "ButtonMid", GUILayout.Height(30f)))
			{
				StopParticleSystem();
			}
			if(GUILayout.Button("STOP/CLEAR EFFECT", "ButtonRight", GUILayout.Height(30f)))
			{
				StopParticleSystem(true);
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(6f);
			
			//----------------------------------------------------------------
			// PROPERTIES
			
			propsFoldout = GUILayout.Toggle(propsFoldout, "EDIT PROPERTIES", EditorStyles.toolbarButton);
			if(!propsFoldout && editingScale)
			{
				StopEditingScaleDialog();
			}
			if(propsFoldout)
			{
				//Background
				Rect vertRect = EditorGUILayout.BeginVertical();
				vertRect.xMax+=2;
				vertRect.xMin--;
				GUI.Box(vertRect, "", (GUIStyle)"RL Background");
				GUILayout.Space(4f);
				//--------
				// Mobile
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				bool newBool = EditorGUILayout.Toggle(new GUIContent("Use Mobile Atlas", "Will use a single texture atlas for all materials, useful to reduce drawcalls"), shfx_variants.isMobile);
				if(newBool != shfx_variants.isMobile)
				{
					Undo.RecordObject(shfx_variants, string.Format("ShapeFX: {0} mobile atlas", newBool ? "enable":"disable"));

					shfx_variants.isMobile = newBool;
					foreach(ParticleEffect effect in particleEffects)
					{
						if(shfx_variants.isMobile)
							effect.ConvertToMobile();
						else
							effect.ConvertToDesktop();
					}
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				//--------
				// Effect Density
				bool guiEnabled = GUI.enabled;
				// enable/disable
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				EditorGUI.BeginChangeCheck();
				bool density = EditorGUILayout.Toggle(new GUIContent("Use Density", "Will automatically adjust the emission rate according to the GameObject's scale"), shfx_variants.useDensity);
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(shfx_variants, density ? "ShapeFX: enable density" : "ShapeFX: disable density");
					shfx_variants.useDensity = density;
					shfx_variants.densityCachedScale = shfx_variants.transform.localScale;
					EditorUtility.SetDirty(shfx_variants);
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				float newValue;
				if(shfx_variants.useDensity)
				{
					string infoLabel = string.Format("{0}\t{1}\nEmission Rates:", "Scale Volume:".PadRight(30), shfx_variants.densityCachedScaleVolume);
					
					GUILayout.Label("Density Values for each effect:");
					for(int i = 0; i < AllEffectDensities.Length; i++)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(6f);
						EditorGUI.BeginChangeCheck();
						
						//toggle
						string displayName = (i == 0) ? shfx_variants.mainName : AllEffectDensities[i].particleSystem.name;
						bool enableDensity = GUILayout.Toggle(AllEffectDensities[i].useDensity, "");
						GUI.enabled &= AllEffectDensities[i].useDensity;
						GUILayout.Label(displayName, EditorStyles.label);
						
						//density value
						var effectDensity = AllEffectDensities[i];
						newValue = EditorGUILayout.FloatField("", AllEffectDensities[i].density, GUILayout.Width(60f));
						if(EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(shfx_variants, "ShapeFX: set density for " + displayName);
							shfx_variants.effectDensities[i].useDensity = enableDensity;
							shfx_variants.effectDensities[i].density = newValue;
							shouldUpdateEmission = true;
							EditorUtility.SetDirty(shfx_variants);
							PreviewParticleSystem();
						}
						
						GUI.enabled = guiEnabled;
						GUILayout.Space(6f);
						EditorGUILayout.EndHorizontal();
						
						infoLabel += string.Format("\n{0}\t{1}", displayName.PadRight(30), effectDensity.densityCachedEmissionRate);
					}
					
					EditorGUILayout.HelpBox(infoLabel, MessageType.Info);
				}
				//--------
				// Effect Speed
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				EditorGUI.BeginChangeCheck();
#if UNITY_5_5_OR_NEWER
				newValue = EditorGUILayout.FloatField("Effect Speed", particleSystem.main.simulationSpeed);
#else
				newValue = EditorGUILayout.FloatField("Effect Speed", particleSystem.playbackSpeed);
#endif
				if(EditorGUI.EndChangeCheck())
				{
#if UNITY_5_5_OR_NEWER
					if (newValue != particleSystem.main.simulationSpeed)
#else
					if (newValue != particleSystem.playbackSpeed)
#endif
					{
						List<Object> objectsToRecord = new List<Object>(allParticleSystem);
						if(lightEffect != null)
						{
							objectsToRecord.Add(lightEffect);
						}
						
						Undo.RecordObjects(objectsToRecord.ToArray(), "ShapeFX: change effect speed");
						foreach(ParticleSystem ps in allParticleSystem)
						{
#if UNITY_5_5_OR_NEWER
							var main = ps.main;
							main.simulationSpeed = newValue;
#else
							ps.playbackSpeed = newValue;
#endif
							EditorUtility.SetDirty(ps);
						}
						if(lightEffect != null)
						{
							lightEffect.playbackSpeed = newValue;
							EditorUtility.SetDirty(lightEffect);
						}
						PreviewParticleSystem();
					}
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				//--------
				// Effect Size
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				EditorGUI.BeginChangeCheck();
				string effectSizeLabel = string.Format("Effect Size{0}", editingScale ? "*":"");
				newValue = EditorGUILayout.FloatField(new GUIContent(effectSizeLabel), scaleValue);
				GUILayout.Space(6f);
				if(GUILayout.Button("×2", EditorStyles.miniButtonLeft, GUILayout.Width(40f)))
				{
					newValue *= 2f;
				}
				if(GUILayout.Button("÷2", EditorStyles.miniButtonRight, GUILayout.Width(40f)))
				{
					newValue /= 2f;
				}
				if(EditorGUI.EndChangeCheck())
				{
					if(!editingScale)
					{
						StartEditingScale();
					}

					if(newValue != scaleValue)
					{
						scaleValue = newValue;

						for(int i = 0; i < particleEffects.Length; i++)
						{
							particleEffects[i].SetScaleValues(scaleValue);
						}

						PreviewParticleSystem();
					}
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUI.enabled &= editingScale;
				if(GUILayout.Button("Revert", ButtonRevert, GUILayout.Width(100f)))
				{
					StopEditingScale(true);
				}
				if(GUILayout.Button("Apply", ButtonApply, GUILayout.Width(100f)))
				{
					StopEditingScale();
				}
				GUI.enabled = guiEnabled;
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();

				//--------
				
				GUILayout.Space(10f);
				EditorGUILayout.EndVertical();
			}

			GUILayout.Space(4f);
			
			//----------------------------------------------------------------
			// SHAPES & STYLES

			shapesFoldout = GUILayout.Toggle(shapesFoldout, "EDIT SHAPES", EditorStyles.toolbarButton);
			if(shapesFoldout)
			{
				//Background
				Rect vertRect = EditorGUILayout.BeginVertical();
				vertRect.xMax+=2;
				vertRect.xMin--;
				GUI.Box(vertRect, "", (GUIStyle)"RL Background");
				GUILayout.Space(4f);

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				EditorGUILayout.LabelField("ParticleSystem", EditorStyles.boldLabel, GUILayout.Width(TBL_LABEL_WIDTH));
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel, GUILayout.MaxWidth(TBL_SHAPE_MAXWIDTH), GUILayout.MinWidth(TBL_SHAPE_MINWIDTH));
				EditorGUILayout.LabelField("Style", EditorStyles.boldLabel, GUILayout.MaxWidth(TBL_STYLE_MAXWIDTH), GUILayout.MinWidth(TBL_STYLE_MINWIDTH));
				EditorGUILayout.LabelField("Blend Mode", EditorStyles.boldLabel, GUILayout.MaxWidth(TBL_BLENDMODE_MAXWIDTH), GUILayout.MinWidth(TBL_BLENDMODE_MINWIDTH));
				EditorGUILayout.LabelField(new GUIContent("SP", "Enable Soft Particles"), EditorStyles.boldLabel, GUILayout.Width(24f));
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				for(int i = 0; i < particleEffects.Length; i++)
				{
					if(ParticleEffect.DrawGUI(particleEffects[i]))
					{
						particleEffects[i].UpdateMaterial();
						this.Repaint();
						PreviewParticleSystem();
					}
				}
				//----------------------------------------------------------------
				//Edit all
				if(particleEffects != null && particleEffects.Length > 1)
				{
					GUILayout.Space(4f);
					bool p_allSP = allSP;
					if(ParticleEffect.DrawGUI("EDIT ALL", ref allShape, ref allStyle, ref allBlendMode, ref allSP, InlineBoldLabel, new string[]{"Shape","Style","Blend Mode"}))
					{
						for(int i = 0; i < particleEffects.Length; i++)
						{
							if(allShape != ParticleEffect.Shape.Unknown)
								particleEffects[i].shape = allShape;
							if(allStyle != ParticleEffect.Style.Unknown)
								particleEffects[i].style = allStyle;
							if(allBlendMode != ParticleEffect.BlendMode.Unknown)
								particleEffects[i].blendMode = allBlendMode;

							if(p_allSP != allSP)
								particleEffects[i].noSoftParticles = allSP;

							particleEffects[i].UpdateMaterial();
						}

						allShape = ParticleEffect.Shape.Unknown;
						allStyle = ParticleEffect.Style.Unknown;
						allBlendMode = ParticleEffect.BlendMode.Unknown;

						GetEffects();
						this.Repaint();
						PreviewParticleSystem();
					}
				}
				//----------------------------------------------------------------
				//Randomize
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				bool guiEnabled = GUI.enabled;
				GUI.enabled &= shfx_variants.rndShape || shfx_variants.rndStyle || shfx_variants.rndBlendMode;
				if(GUILayout.Button("Randomize", EditorStyles.miniButton, GUILayout.Width(TBL_LABEL_WIDTH - 20f)))
				{
					for(int i = 0; i < particleEffects.Length; i++)
					{
						particleEffects[i].Randomize(shfx_variants.rndShape, shfx_variants.rndStyle, shfx_variants.rndBlendMode);
						particleEffects[i].UpdateMaterial();
						this.Repaint();
						PreviewParticleSystem();
					}
				}
				GUI.enabled = guiEnabled;
				GUILayout.Space(20f);
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				shfx_variants.rndShape = GUILayout.Toggle(shfx_variants.rndShape, "", GUILayout.MaxWidth(TBL_SHAPE_MAXWIDTH), GUILayout.MinWidth(TBL_SHAPE_MINWIDTH));
				shfx_variants.rndStyle = GUILayout.Toggle(shfx_variants.rndStyle, "", GUILayout.MaxWidth(TBL_STYLE_MAXWIDTH), GUILayout.MinWidth(TBL_STYLE_MINWIDTH));
				shfx_variants.rndBlendMode = GUILayout.Toggle(shfx_variants.rndBlendMode, "", GUILayout.MaxWidth(TBL_BLENDMODE_MAXWIDTH), GUILayout.MinWidth(TBL_BLENDMODE_MINWIDTH));
				GUILayout.Space(26f);
				if(EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(shfx_variants);
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(10f);

				EditorGUILayout.EndVertical();
			}

			GUILayout.Space(4f);

			//----------------------------------------------------------------
			// COLOR CORRECTION

			string editColorLabel = string.Format("EDIT COLORS{0}", editingColors ? "*":"");
			colorsFoldout = GUILayout.Toggle(colorsFoldout, editColorLabel, EditorStyles.toolbarButton);
			if(!colorsFoldout && editingColors)
			{
				StopEditingColorsDialog();
			}
			if(colorsFoldout)
			{
				//Background
				Rect vertRect = EditorGUILayout.BeginVertical();
				vertRect.xMax+=2;
				vertRect.xMin--;
				GUI.Box(vertRect, "", (GUIStyle)"RL Background");
				GUILayout.Space(4f);

				bool colorEditChanged = false;

				// Hue
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Hue", EditorStyles.label, GUILayout.Width(TBL_LABEL_WIDTH));
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				hueShift = EditorGUILayout.Slider(hueShift, -180f, 180f, GUILayout.MaxWidth(TBL_BLENDMODE_MAXWIDTH+TBL_SHAPE_MAXWIDTH+TBL_STYLE_MAXWIDTH+8));
				if(EditorGUI.EndChangeCheck())
				{
					colorEditChanged = true;
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();
				
				// Saturation
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Saturation", EditorStyles.label, GUILayout.Width(TBL_LABEL_WIDTH));
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				saturationShift = EditorGUILayout.Slider(saturationShift, -1f, 1f, GUILayout.MaxWidth(TBL_BLENDMODE_MAXWIDTH+TBL_SHAPE_MAXWIDTH+TBL_STYLE_MAXWIDTH+8));
				if(EditorGUI.EndChangeCheck())
				{
					colorEditChanged = true;
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();

				// Value
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Value", EditorStyles.label, GUILayout.Width(TBL_LABEL_WIDTH));
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				valueShift = EditorGUILayout.Slider(valueShift, -1f, 1f, GUILayout.MaxWidth(TBL_BLENDMODE_MAXWIDTH+TBL_SHAPE_MAXWIDTH+TBL_STYLE_MAXWIDTH+8));
				if(EditorGUI.EndChangeCheck())
				{
					colorEditChanged = true;
				}
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();

				// Change
				if(colorEditChanged)
				{
					if(!editingColors)
					{
						StartEditingColors();
					}

					if(hueShift == 0f && saturationShift == 0f && valueShift == 0f)
					{
						StopEditingColors(true);
					}
					else
					{
						for(int i = 0; i < particleEffects.Length; i++)
						{
							float hue = hueShift/360f;
							particleEffects[i].AdjustColors(hue, saturationShift, valueShift, forceSaturation, affectLight);
						}

						if(colorsPreviewParticles || !particleSystem.isPlaying)
						{
							PreviewParticleSystem();
						}
					}

					//try to update gradient previews using reflection
					TryUpdateGradientsPreview();
				}

				GUILayout.Space(4f);

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(6f);
				if(GUILayout.Button("Invert Colors", EditorStyles.miniButton, GUILayout.Width(100f)))
				{
					for(int i = 0; i < particleEffects.Length; i++)
					{
						particleEffects[i].InvertColors(affectLight);
						PreviewParticleSystem();
					}
				}

				GUILayout.FlexibleSpace();
				bool guiEnabled = GUI.enabled;
				GUI.enabled &= editingColors;
				if(GUILayout.Button("Revert", ButtonRevert, GUILayout.Width(100f)))
				{
					StopEditingColors(true);
				}
				if(GUILayout.Button("Apply", ButtonApply, GUILayout.Width(100f)))
				{
					StopEditingColors();
				}
				GUI.enabled = guiEnabled;
				GUILayout.Space(6f);
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(4f);
				forceSaturation = GUILayout.Toggle(forceSaturation, new GUIContent("Force Saturation", "Force saturation changes on grayscale colors\nUseful if the effect is white for example"), GUILayout.ExpandWidth(false));
				if(this.lightEffect == null)
				{
					GUI.enabled = false;
					affectLight = GUILayout.Toggle(affectLight, new GUIContent("Affect Light", "Affect attached light if it exists\n\nUnavailable because:\nEffect doesn't have a light attached"), GUILayout.ExpandWidth(false));
					GUI.enabled = guiEnabled;
				}
				else if(this.lightEffect.colorFromParticleSystem)
				{
					GUI.enabled = false;
					affectLight = GUILayout.Toggle(affectLight, new GUIContent("Affect Light", "Affect attached light if it exists\n\nUnavailable because:\nLight effect color is linked to the Particle System"), GUILayout.ExpandWidth(false));
					GUI.enabled = guiEnabled;
				}
				else
					affectLight = GUILayout.Toggle(affectLight, new GUIContent("Affect Light", "Affect attached light if it exists"), GUILayout.ExpandWidth(false));
				colorsPreviewParticles = GUILayout.Toggle(colorsPreviewParticles, new GUIContent("Restart Particle System on change", "Restart Particle System when editing colors"), GUILayout.ExpandWidth(false));

				GUILayout.Space(10f);
				EditorGUILayout.EndVertical();
			}

			GUILayout.Space(4f);

			//----------------------------------------------------------------
			// LIGHT EFFECT

			if(lightEffect != null)
			{
				bool lightEnabled = lightEffect.gameObject.activeSelf;
				if(lightEnabled && lightEffect.linkedEffect != this.particleSystem)
				{
					lightEffect.linkedEffect = this.particleSystem;
					EditorUtility.SetDirty(lightEffect);
				}

				if(!lightEffect.autoPlayFromParticleSystem)
				{
					lightEffect.autoPlayFromParticleSystem = true;
					EditorUtility.SetDirty(lightEffect);
				}

				if(lightEffect.colorFromParticleSystem)
				{

#if UNITY_5_5_OR_NEWER
					if(!(lightEffect.lightComponent.color.r == lightEffect.linkedEffect.main.startColor.color.r
					   && lightEffect.lightComponent.color.g == lightEffect.linkedEffect.main.startColor.color.g
					   && lightEffect.lightComponent.color.b == lightEffect.linkedEffect.main.startColor.color.b))
#else
					if(!(lightEffect.lightComponent.color.r == lightEffect.linkedEffect.startColor.r
					   && lightEffect.lightComponent.color.g == lightEffect.linkedEffect.startColor.g
					   && lightEffect.lightComponent.color.b == lightEffect.linkedEffect.startColor.b))
#endif
					{
#if UNITY_5_5_OR_NEWER
						Color targetColor = lightEffect.linkedEffect.main.startColor.color;
#else
						Color targetColor = lightEffect.linkedEffect.startColor;
#endif
						targetColor.a = 1f;
						lightEffect.lightComponent.color = targetColor;
						EditorUtility.SetDirty(lightEffect.lightComponent);
					}
				}

				lightFoldout = GUILayout.Toggle(lightFoldout, "EDIT LIGHT", EditorStyles.toolbarButton);
				if(lightFoldout)
				{
					//Background
					Rect vertRect = EditorGUILayout.BeginVertical();
					vertRect.xMax+=2;
					vertRect.xMin--;
					GUI.Box(vertRect, "", (GUIStyle)"RL Background");
					GUILayout.Space(4f);


					lightEnabled = GUILayout.Toggle(lightEnabled, "Enable Light Effect");
					if(lightEffect.gameObject.activeSelf != lightEnabled)
					{
						Undo.RecordObject(lightEffect.gameObject, "ShapeFX: disable light effect");
						lightEffect.gameObject.SetActive(lightEnabled);
						EditorUtility.SetDirty(lightEffect.gameObject);
						PreviewParticleSystem();
					}

					bool guiEnabled = GUI.enabled;
					GUI.enabled &= lightEnabled;
					EditorGUI.BeginChangeCheck();

					SerializedObject effectLightSo = new SerializedObject(lightEffect);
					SerializedProperty prop_maxIntensity = effectLightSo.FindProperty("peakIntensity");
					SerializedProperty prop_delay = effectLightSo.FindProperty("delay");
					SerializedProperty prop_duration = effectLightSo.FindProperty("duration");
					SerializedProperty prop_loop = effectLightSo.FindProperty("loop");
					SerializedProperty prop_fadeIn = effectLightSo.FindProperty("fadeIn");
					SerializedProperty prop_fadeOut = effectLightSo.FindProperty("fadeOut");
					SerializedProperty prop_intensityCurve = effectLightSo.FindProperty("intensityCurve");
					SerializedProperty prop_linkedColor = effectLightSo.FindProperty("colorFromParticleSystem");
					SerializedProperty prop_useGradient = effectLightSo.FindProperty("useColorGradient");
					SerializedProperty prop_gradient = effectLightSo.FindProperty("colorGradient");
					SerializedProperty prop_cachedColor = effectLightSo.FindProperty("cachedColor");

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_maxIntensity);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_loop);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					GUI.enabled &= prop_loop.boolValue;
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_fadeIn);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_fadeOut);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					GUI.enabled = guiEnabled && lightEnabled;
					
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_delay);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_duration);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_intensityCurve);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					bool prop_linkedColor_previous = prop_linkedColor.boolValue;
					EditorGUILayout.PropertyField(prop_linkedColor, new GUIContent("Color from PS", "Color from Particle System"));
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					GUI.enabled &= !prop_linkedColor.boolValue;
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					bool prop_useGradient_previous = prop_useGradient.boolValue;
					EditorGUILayout.PropertyField(prop_useGradient);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();
					GUI.enabled = guiEnabled && lightEnabled;

					//Light shortcuts
					SerializedObject lightSo = new SerializedObject(lightEffect.lightComponent);
					SerializedProperty prop_lightColor = lightSo.FindProperty("m_Color");
					SerializedProperty prop_lightRange = lightSo.FindProperty("m_Range");

					// restore previous light color if reverted from linked color or gradient color
					if((prop_linkedColor_previous != prop_linkedColor.boolValue)
					 ||(prop_useGradient_previous != prop_useGradient.boolValue))
					{
						if(editingColors)
						{
							StopEditingColorsDialog();
						}

						prop_lightColor.colorValue = lightEffect.cachedColor;
						lightSo.ApplyModifiedProperties();
					}

					GUI.enabled &= !prop_linkedColor.boolValue;
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					if(!prop_linkedColor.boolValue && prop_useGradient.boolValue)
						EditorGUILayout.PropertyField(prop_gradient);
					else
						EditorGUILayout.PropertyField(prop_lightColor);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();
					GUI.enabled = guiEnabled && lightEnabled;
					
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6f);
					EditorGUILayout.PropertyField(prop_lightRange);
					GUILayout.Space(6f);
					EditorGUILayout.EndHorizontal();

					//--------

					if(EditorGUI.EndChangeCheck())
					{
						if(!prop_linkedColor.boolValue && !prop_useGradient.boolValue)
						{
							prop_cachedColor.colorValue = prop_lightColor.colorValue;
						}

						effectLightSo.ApplyModifiedProperties();
						lightSo.ApplyModifiedProperties();
						PreviewParticleSystem();
					}

					GUI.enabled = guiEnabled;

					GUILayout.Space(10f);
					EditorGUILayout.EndVertical();
				}
			}

			//----------------------------------------------------------------

#if SHFX_DEBUG
		DEBUG_PANEL:
			GUILayout.Space(10f);
			GUILayout.BeginVertical("ColorPickerBox");
			GUILayout.Label("DEBUG", EditorStyles.boldLabel);
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("mainName"));
			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}

			GUILayout.Space(4f);
			GUILayout.EndVertical();
			GUILayout.Space(10f);
#endif
		}

		//--------------------------------------------------------------------------------------------------------------------------------

		private void GetEffects()
		{
			List<ParticleEffect> effects = new List<ParticleEffect>();
			effects.Add(new ParticleEffect(particleSystem, shfx_variants.mainName, shfx_variants.isMobile));
			for(int i = 0; i < shfx_variants.transform.childCount; i++)
			{
				ParticleSystem subEffect = shfx_variants.transform.GetChild(i).GetComponent<ParticleSystem>();
				if(subEffect != null)
				{
					effects.Add(new ParticleEffect(subEffect, subEffect.name, shfx_variants.isMobile));
				}
			}
			particleEffects = effects.ToArray();
		}

		private bool TryUpdateGradientsPreview()
		{
			System.Reflection.Assembly editorgGuiLayoutAssembly = typeof(EditorGUILayout).Assembly;
			if(editorgGuiLayoutAssembly != null)
			{
				System.Type gpc = editorgGuiLayoutAssembly.GetType("UnityEditorInternal.GradientPreviewCache");
				if(gpc != null)
				{
					System.Reflection.MethodInfo clearCache = gpc.GetMethod("ClearCache", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
					if(clearCache != null)
					{
						clearCache.Invoke(null, null);
						return true;
					}
				}
			}

			return false;
		}

		// COLOR EDITING
		private void StartEditingColors()
		{
			if(editingColors)
			{
				Debug.LogWarning("ShapeFX - StartEditingColors() => editingColors already true");
			}

			for(int i = 0; i < particleEffects.Length; i++)
			{
				particleEffects[i].GetOriginalColors();
			}

			editingColors = true;
		}

		private void StopEditingColorsDialog()
		{
			if(EditorUtility.DisplayDialog("ShapeFX Variants - Color Edit", "You have pending color edits!", "Apply", "Revert"))
			{
				StopEditingColors();
			}
			else
			{
				StopEditingColors(true);
			}
		}

		private void StopEditingColors(bool revert = false)
		{
			if(!editingColors) return;

			if(revert)
			{
				for(int i = 0; i < particleEffects.Length; i++)
				{
					particleEffects[i].RevertColors();
				}
			}

			hueShift = 0f;
			saturationShift = 0f;
			valueShift = 0f;

			if(colorsPreviewParticles || !particleSystem.isPlaying)
			{
				PreviewParticleSystem();
			}

			editingColors = false;

			//try to update gradient previews using reflection
			TryUpdateGradientsPreview();
		}

		// SCALE EDITING
		private void StartEditingScale()
		{
			if(editingScale)
			{
				Debug.LogWarning("ShapeFX - StartEditingScale() => editingScale already true");
			}

			foreach(ParticleEffect effect in particleEffects)
			{
				effect.GetScaleValues(effect == particleEffects[0]);
			}

			editingScale = true;
		}
		
		private void StopEditingScaleDialog()
		{
			if(scaleValue != 1 && EditorUtility.DisplayDialog("ShapeFX Variants - Scale",
			                      string.Format("You have pending scale edits!\n\nYou changed by {1} the size of effect\n'{0}'",shfx_variants.name,scaleValue), "Apply", "Revert"))
			{
				StopEditingScale();
			}
			else
			{
				StopEditingScale(true);
			}
		}
		
		private void StopEditingScale(bool revert = false)
		{
			if(!editingScale) return;
			
			if(revert)
			{
				for(int i = 0; i < particleEffects.Length; i++)
				{
					particleEffects[i].SetScaleValues(1f);
				}
			}

			PreviewParticleSystem();

			editingScale = false;
			scaleValue = 1f;
		}

		// PROPERTIES EDITING

		private bool ShouldUpdateEmission(ShFX_Variants.EffectDensity effectDensity, out bool emissionChanged)
		{
			SerializedObject particleSystemSO = new SerializedObject(effectDensity.particleSystem);
			particleSystemSO.Update();
			SerializedProperty emissionRateProp = particleSystemSO.FindProperty(strEmissionModuleRateScalar);
			emissionChanged = (effectDensity.densityCachedEmissionRate != emissionRateProp.floatValue);
			return shouldUpdateEmission || (shfx_variants.transform.localScale != shfx_variants.densityCachedScale) || emissionChanged;
		}

		private void UpdateEmissionDensity(ShFX_Variants.EffectDensity effectDensity, bool emissionChanged)
		{
			//update emission value
			SerializedObject particleSystemSO = new SerializedObject(effectDensity.particleSystem);
			particleSystemSO.Update();
			SerializedProperty emissionRateProp = particleSystemSO.FindProperty(strEmissionModuleRateScalar);

			float scaleVolume = shfx_variants.transform.localScale.x * shfx_variants.transform.localScale.y * shfx_variants.transform.localScale.z;
			if(emissionChanged)
			{
				effectDensity.density = emissionRateProp.floatValue/scaleVolume;
			}
			else
			{
				emissionRateProp.floatValue = scaleVolume * effectDensity.density;
				particleSystemSO.ApplyModifiedProperties();
			}

			//update cached values
			effectDensity.densityCachedEmissionRate = emissionRateProp.floatValue;
			EditorUtility.SetDirty(shfx_variants);
		}

		// MISC
		private void PreviewParticleSystem()
		{
			//preview is delayed to not interfere with the default ParticleSystem inspector
			previewFrameCount = 0;
			EditorApplication.update -= EditorUpdate_EffectPreview;
			EditorApplication.update += EditorUpdate_EffectPreview;
		}

		private void StopParticleSystem(bool clear = false)
		{
			if(particleSystem != null)
			{
				particleSystem.Stop();
				if(clear)
					particleSystem.Clear();

				var effectLight = GetLightEffect(shfx_variants);
				if(effectLight != null)
					effectLight.StopLightEffect(clear);
			}
		}

		int previewFrameCount = 0;
		void EditorUpdate_EffectPreview()
		{
			previewFrameCount++;

			if(previewFrameCount >= 6)
			{
				if(particleSystem != null)
				{
					particleSystem.Play();
					var effectLight = GetLightEffect(shfx_variants);
					if(effectLight != null)
						effectLight.Editor_RestartLightEffect();
				}
				EditorApplication.update -= EditorUpdate_EffectPreview;
			}
			else if(previewFrameCount == 2)
			{
				if(particleSystem != null)
				{
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------
		// COLOR UTILITIES

		private struct ColorHSV
		{
			public float h;
			public float s;
			public float v;

			public bool forceSaturation;

			public ColorHSV(float hue, float saturation, float value)
			{
				h = hue;
				s = saturation;
				v = value;

				forceSaturation = false;
			}
		}

		static private ColorHSV RGBtoHSV(Color color)
		{
			ColorHSV hsv;
#if UNITY_5_2
			EditorGUIUtility.RGBToHSV(color, out hsv.h, out hsv.s, out hsv.v);
#else
			Color.RGBToHSV(color, out hsv.h, out hsv.s, out hsv.v);
#endif
			hsv.forceSaturation = false;
			return hsv;
		}
		
		static private Color HSVtoRGB(ColorHSV hsv)
		{
#if UNITY_5_2
			return EditorGUIUtility.HSVToRGB(hsv.h, hsv.s, hsv.v);
#else
			return Color.HSVToRGB(hsv.h, hsv.s, hsv.v);
#endif
		}

		//--------------------------------------------------------------------------------------------------------------------------------
		// GUI

		// Custom styles

		static public GUIStyle _LineStyle;
		static public GUIStyle LineStyle
		{
			get
			{
				if(_LineStyle == null)
				{
					_LineStyle = new GUIStyle();
					_LineStyle.normal.background = EditorGUIUtility.whiteTexture;
					_LineStyle.stretchWidth = true;
				}
				
				return _LineStyle;
			}
		}

		static public GUIStyle _InlineBoldLabel;
		static public GUIStyle InlineBoldLabel
		{
			get
			{
				if(_InlineBoldLabel == null)
				{
					_InlineBoldLabel = new GUIStyle(EditorStyles.label);
					_InlineBoldLabel.font = EditorStyles.boldFont;
				}
				return _InlineBoldLabel;
			}
		}

		static public GUIStyle _buttonRevert;
		static public GUIStyle ButtonRevert
		{
			get
			{
				if(_buttonRevert == null)
				{
					_buttonRevert = new GUIStyle(EditorStyles.miniButtonLeft);
					_buttonRevert.normal.textColor = EditorGUIUtility.isProSkin ? new Color(1f,.6f,.1f) : new Color(.5f,0f,0f);
					_buttonRevert.active.textColor = EditorGUIUtility.isProSkin ? new Color(1f,.8f,.4f) : new Color(.9f,0f,0f);
				}
				return _buttonRevert;
			}
		}

		static public GUIStyle _buttonApply;
		static public GUIStyle ButtonApply
		{
			get
			{
				if(_buttonApply == null)
				{
					_buttonApply = new GUIStyle(EditorStyles.miniButtonRight);
					_buttonApply.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0f,.8f,0f) : new Color(0f,.6f,0f);
					_buttonApply.active.textColor = EditorGUIUtility.isProSkin ? new Color(0f,1f,0f) : new Color(.5f,1f,.5f);
				}
				return _buttonApply;
			}
		}

		static public Dictionary<Color, GUIStyle> _ColoredLabels = new Dictionary<Color, GUIStyle>();
		static public GUIStyle GetColoredLabel(Color color)
		{
			if(!_ColoredLabels.ContainsKey(color))
			{
				GUIStyle newColoredLabel = new GUIStyle(EditorStyles.label);
				newColoredLabel.normal.textColor = color;
				_ColoredLabels.Add(color, newColoredLabel);
			}

			return _ColoredLabels[color];
		}

		static public Dictionary<Color, GUIStyle> _ColoredBoldLabels = new Dictionary<Color, GUIStyle>();
		static public GUIStyle GetColoredLabelBold(Color color)
		{
			if(!_ColoredBoldLabels.ContainsKey(color))
			{
				GUIStyle newColoredLabel = new GUIStyle(EditorStyles.boldLabel);
				newColoredLabel.normal.textColor = color;
				_ColoredBoldLabels.Add(color, newColoredLabel);
			}
			
			return _ColoredBoldLabels[color];
		}


		// Tools

		static public void GUISeparator()
		{
			GUILayout.Space(4f);
			GUILine(new Color(.6f,.6f,.6f), 1);
			GUILayout.Space(4f);
		}
		
		static public void GUILine(float height = 2f)
		{
			GUILine(Color.black, height);
		}
		static public void GUILine(Color color, float height = 2f)
		{
			Rect position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);
			
			if(Event.current.type == EventType.Repaint)
			{
				Color orgColor = GUI.color;
				GUI.color = orgColor * color;
				LineStyle.Draw(position, false, false, false, false);
				GUI.color = orgColor;
			}
		}
	}
}

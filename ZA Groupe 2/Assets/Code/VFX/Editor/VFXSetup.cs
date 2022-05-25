using UnityEditor;
using UnityEngine;

public class VFXSetup : EditorWindow
{
    //Text in Button
    private readonly string createMasterParticules = "New Project Shell";
    private readonly string memorizeMaster = "Memorize as Master";
    private readonly string particuleChildShell = "New System Shell";

    //Text Toolbar
    private readonly string[] versionRender = {"SRP", "URP", "HDRP"};

    //private Material
    private Material _basicMaterial;
    private Material _customMaterial;

    //Private Int 
    private int _rendererInt;
    private string changeProjectHeader;
    private Transform[] children;
    private GameObject childVFX;

    //Object Selected
    private Transform masterPosition;
    private GameObject objectSelected;
    private string objectSelectedExtension;

    private bool isCreateChildParticules = false;

    //Bool Particules
    private bool isCreateMasterParticules = false;
    private bool useCustomMaterial = false;

    //Particules System
    private ParticleSystemRenderer _renderer;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.ShapeModule shape;


    //Private Transform/Object
    private GameObject masterVFX;
    private string particuleSystemName;

    //TEXT in the Label;
    private string projectNameHeader;
    private string projectMemoriseHeader;


    private void OnGUI()
    {
        //Create Project
        GUILayout.Label("Create Projet", EditorStyles.boldLabel);
        projectNameHeader = EditorGUILayout.TextField("Project Name ", projectNameHeader);
        //BUTTON CREATE MASTER
        if (GUILayout.Button(createMasterParticules)) SpawnParticulesTransform(true);
        EditorGUILayout.Space();


        //Change Project
        GUILayout.Label("Change Project", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Project on : " + projectNameHeader);
        EditorGUILayout.LabelField("Object Select On : " + projectMemoriseHeader);
        //BUTTON MEMORIZE MASTER
        if (GUILayout.Button(memorizeMaster)) MemorizeMaster();
        EditorGUILayout.Space();

        //New Particules System
        GUILayout.Label("Particle System Name", EditorStyles.boldLabel);
        particuleSystemName = EditorGUILayout.TextField("Particule System Name ", particuleSystemName);
        //Insert Button
        if (GUILayout.Button(particuleChildShell)) SpawnParticulesTransform();
        EditorGUILayout.Space();

        //KeyFrame 
        GUILayout.Label("KeyFrames", EditorStyles.boldLabel);
        Time.timeScale = EditorGUILayout.Slider(" Modify Time", Time.timeScale, 0.01f, 2f);
        EditorGUILayout.Space();

        //URP/HDRP/RenderPipeline
        GUILayout.Label("Renderer", EditorStyles.boldLabel);
        _rendererInt = GUILayout.Toolbar(_rendererInt, versionRender);
        useCustomMaterial = EditorGUILayout.Toggle("Use Custom Material", useCustomMaterial);

        if (useCustomMaterial)
        {
            _customMaterial = (Material) EditorGUILayout.ObjectField("Material", _customMaterial, typeof(Material));
        }
        else if (_rendererInt >= 0)
        {
            SwitchMaterialBasic();
            EditorGUILayout.LabelField("Basic Material :" + _basicMaterial.name);
        }
        
        /*
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();
        */
    }


    // Add menu item named "My Window" to the Window menu
    [MenuItem("Tools/VFX/VFX Particules Setup")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow<VFXSetup>(" VFX Particules Setup");
    }

    private void SpawnParticulesTransform(bool firstSetup = false)
    {
        if (firstSetup)
        {
            //Master FX
            masterVFX = new GameObject();
            var posOrigin = new Vector3(0, 0, 0);
            masterVFX.transform.position = posOrigin;
            masterVFX.name = string.IsNullOrWhiteSpace(projectNameHeader) ? "Master_VFX_001" : projectNameHeader;

            //Child Object
            childVFX = Instantiate(masterVFX, masterVFX.transform);
            childVFX.name = string.IsNullOrWhiteSpace(particuleSystemName) ? "PS_FX_001" : particuleSystemName;

            //Add Particule Setup
            ParticulesSystemSetup(masterVFX);
            ParticulesSystemSetup(childVFX, true);
        }
        else
        {
            NameParticulesSystem(particuleSystemName, childVFX, objectSelected != null ? objectSelected.transform : masterPosition);
        }
    }

    private void ParticulesSystemSetup(GameObject objFX, bool child = false)
    {
        //Add Module Property
        objFX.AddComponent<ParticleSystem>();
        var particuleComponent = objFX.GetComponent<ParticleSystem>();
        var particuleRendererComponent = objFX.GetComponent<ParticleSystemRenderer>();

        //Link Module
        mainModule = particuleComponent.main;
        emission = particuleComponent.emission;
        shape = particuleComponent.shape;
        _renderer = particuleRendererComponent;

        if (!child)
        {
            //Enabled Property Module
            emission.enabled = false;
            shape.enabled = false;
            shape.enabled = false;
            _renderer.enabled = false;
            _renderer.material = null;

            //Set Value Property
            mainModule.duration = 2f;
            mainModule.startLifetime = 2f;
            mainModule.startSpeed = 2f;
        }

        SwitchMaterialBasic();
        _renderer.material = _basicMaterial;
    }

    private void MemorizeMaster()
    {
        objectSelected = Selection.activeGameObject;
        if (objectSelected.GetComponent<ParticleSystem>() != null 
            && !objectSelected.GetComponent<ParticleSystemRenderer>().enabled)
        {
            projectMemoriseHeader = Selection.activeTransform.name;
        }
        else if(objectSelected.GetComponent<ParticleSystem>() != null)
        {
            projectMemoriseHeader = "Error, Child Particules System";
        }
        else
        {
            projectMemoriseHeader = "Error not a Particule System Master";
        }
    }

    private void NameParticulesSystem(string name, GameObject instantiateFX, Transform parent)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            instantiateFX = Instantiate(instantiateFX, parent);
            children = parent.GetComponentsInChildren<Transform>();
            if (children.Length > 10)
                instantiateFX.name = "PS_FX" + "_0" + (children.Length - 1);
            else
                instantiateFX.name = "PS_FX" + "_00" + (children.Length - 1);
        }
        else
        {
            instantiateFX = Instantiate(instantiateFX, parent);
            instantiateFX.name = name;
        }
    }

    private void SwitchMaterialBasic()
    {
        switch (_rendererInt)
        {
            case 0:
                _basicMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                Debug.Log("Switch Material");
                break;
            case 1:
                _basicMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                Debug.Log("Switch Material 1 ");
                break;
            case 2:
                _basicMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                Debug.Log("Switch Material 2");
                break;
        }
    }

    private void ProParameterParticulesSystem()
    {
        
    }

    private void ModifyTime()
    {
        
    }
    
}
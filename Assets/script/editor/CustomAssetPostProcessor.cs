using UnityEditor;

public class CustomAssetPostProcessor : AssetPostprocessor {

	void OnPreprocessModel() {
		ModelImporter modelImporter = assetImporter as ModelImporter;
		modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
	}

}

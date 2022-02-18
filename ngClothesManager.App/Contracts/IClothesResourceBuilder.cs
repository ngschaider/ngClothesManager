namespace ngClothesManager.App.Contracts {
    internal interface IClothesResourceBuilder {
        void BuildResource(Project project, string outputFolder, string collectionName);
    }
}

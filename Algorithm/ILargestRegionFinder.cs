namespace ConnectedGrid.Algorithm
{
    public interface ILargestRegionFinder
    {
        (int largestRegion, int[,] mutatedMatrix) GetLargestRegion(int [,] matrix);        
    }
}
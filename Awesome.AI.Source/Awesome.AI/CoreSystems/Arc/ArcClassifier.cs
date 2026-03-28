namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public enum Primitive
    {
        Rotate90,
        Rotate180,
        Rotate270,
        MirrorHorizontal,
        MirrorVertical,
        TranslateDown,
        TranslateRight,
        ColorMap,
        ColorFill,
        CropBoundingBox,
        ExpandCanvas,
        ObjectCopy
    }

    public class ArcSample
    {
        public float[] Features;
        public bool[] Labels;
    }

    public class ArcClassifier
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;

        private float[,] W1;
        private float[] B1;
        private float[,] W2;
        private float[] B2;
        private Random rnd;

        public ArcClassifier(int inputSize, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;
            rnd = new Random();

            W1 = RandomMatrix(inputSize, hiddenSize);
            B1 = new float[hiddenSize];
            W2 = RandomMatrix(hiddenSize, outputSize);
            B2 = new float[outputSize];
        }

        private float[,] RandomMatrix(int rows, int cols)
        {
            float[,] m = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    m[i, j] = (float)(rnd.NextDouble() * 0.2 - 0.1); // [-0.1,0.1]
            return m;
        }

        private void Forward(float[] input, out float[] hiddenOut, out float[] outputOut)
        {
            hiddenOut = new float[hiddenSize];
            for (int j = 0; j < hiddenSize; j++)
            {
                float sum = B1[j];
                for (int i = 0; i < inputSize; i++)
                    sum += input[i] * W1[i, j];
                hiddenOut[j] = 1f / (1f + (float)Math.Exp(-sum));
            }

            outputOut = new float[outputSize];
            for (int k = 0; k < outputSize; k++)
            {
                float sum = B2[k];
                for (int j = 0; j < hiddenSize; j++)
                    sum += hiddenOut[j] * W2[j, k];
                outputOut[k] = 1f / (1f + (float)Math.Exp(-sum));
            }
        }

        public void Train(ArcSample[] samples, int epochs = 1000, float lr = 0.1f)
        {
            for (int e = 0; e < epochs; e++)
            {
                foreach (var sample in samples)
                {
                    Forward(sample.Features, out float[] hidden, out float[] output);

                    float[] deltaOutput = new float[outputSize];
                    for (int k = 0; k < outputSize; k++)
                        deltaOutput[k] = (output[k] - (sample.Labels[k] ? 1f : 0f)) * output[k] * (1 - output[k]);

                    for (int j = 0; j < hiddenSize; j++)
                        for (int k = 0; k < outputSize; k++)
                            W2[j, k] -= lr * deltaOutput[k] * hidden[j];
                    for (int k = 0; k < outputSize; k++)
                        B2[k] -= lr * deltaOutput[k];

                    float[] deltaHidden = new float[hiddenSize];
                    for (int j = 0; j < hiddenSize; j++)
                    {
                        float sum = 0f;
                        for (int k = 0; k < outputSize; k++)
                            sum += W2[j, k] * deltaOutput[k];
                        deltaHidden[j] = sum * hidden[j] * (1 - hidden[j]);
                    }

                    for (int i = 0; i < inputSize; i++)
                        for (int j = 0; j < hiddenSize; j++)
                            W1[i, j] -= lr * deltaHidden[j] * sample.Features[i];
                    for (int j = 0; j < hiddenSize; j++)
                        B1[j] -= lr * deltaHidden[j];
                }
            }
        }

        public float[] Predict(float[] input)
        {
            Forward(input, out _, out float[] output);
            return output;
        }        
    }
}


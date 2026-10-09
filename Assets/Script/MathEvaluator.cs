public static class MathEvaluator
{
    // O Enum universal que todos os scripts vão usar
    public enum Simbolo { Igual, Maior, Menor }

    // A única função de validação que o jogo inteiro vai chamar
    public static bool Validar(int valor1, Simbolo simbolo, int valor2)
    {
        switch (simbolo)
        {
            case Simbolo.Igual: 
                return valor1 == valor2;
            case Simbolo.Maior: 
                return valor1 > valor2;
            case Simbolo.Menor: 
                return valor1 < valor2;
            default: 
                return false;
        }
    }
}
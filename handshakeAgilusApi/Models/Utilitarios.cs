using System;

namespace handshakeAgilusApi.Models
{
    public class Utilitarios
    {
        public static string GerarToken(string codigoCliente)
        {
            return Criptografia.Encriptar(codigoCliente);
        }
        public static string ConexaoSolicitada(string token)
        {
            string codigoCliente = null;
            string tokenDecriptado = null;
            string enderecoServidor = null;
            try
            {
                tokenDecriptado = Criptografia.Decriptar(token);
            }
            catch (Exception)
            {
                throw new Exception("Chave inválida");
            }

            codigoCliente = tokenDecriptado.Split('|')[0];

            try
            {
                enderecoServidor = System.Configuration.ConfigurationManager.ConnectionStrings[codigoCliente].ConnectionString;
            }
            catch
            {
                // Nada a ser feito
            }

            return enderecoServidor;
        }
        private static string Encode(string value)
        {
            var hash = System.Security.Cryptography.MD5.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
}
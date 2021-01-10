using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new Data.ApplicationContext();
            //db.Database.EnsureCreated();

            //var migrations = db.Database.GetPendingMigrations();
            //if (migrations.Any())
            //    db.Database.Migrate();

            //InserirDados();
            //InserirDadosEmMassa();
            //InserirDadosEmMassaList();
            //ConsultarDados();
            //CadastrarPedido();
            //ConsultarPedidoCarregamentoAdiantado();
            //AtualizarDados();

            AtualizarDadosDesconectado();

            Console.ReadKey();
        }

        private static void RemoverDadosDesconectado()
        {
            using var db = new Data.ApplicationContext();
            var cliente = new Cliente
            {
                Id = 1
            };

            //db.Clientes.Remove(cliente);
            //db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }

        private static void RemoverDados()
        {
            using var db = new Data.ApplicationContext();
            var cliente = db.Clientes.Find(2);

            //db.Clientes.Remove(cliente);
            //db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }

        private static void AtualizarDadosDesconectado()
        {
            using var db = new Data.ApplicationContext();

            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new
            {
                Nome = "Cliente desconectado",
                Telefone = "4238427384"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
            db.SaveChanges();
        }

        private static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.Find(1);
            cliente.Nome = "Cliente Alterado Passo 2";

            //db.Clientes.Update(cliente);
            db.SaveChanges();
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db
                .Pedidos
                .Include(p => p.Itens)
                .ThenInclude(p=>p.Produto)
                .ToList();
        }

        private static void CadastrarPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = ValueObject.StatusPedido.Analise,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            db.Pedidos.Add(pedido);

            db.SaveChanges();
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1321354336643",
                Valor = 10m,
                TipoProduto = ValueObject.TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Felipe Augusto",
                CEP = "12345678",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                Telefone = "21999999999"
            };

            using var db = new Data.ApplicationContext();

            db.AddRange(cliente, produto);

            var registros = db.SaveChanges(); // só insere 1x, pois monitora a instancia

            Console.WriteLine($"Total de registros inseridos: {registros}");
        }

        private static void InserirDadosEmMassaList()
        {
            var listaCliente = new[]
            {
                new Cliente
                {
                    Nome = "Felipe Augusto",
                    CEP = "12345678",
                    Cidade = "Rio de Janeiro",
                    Estado = "RJ",
                    Telefone = "21999999999"
                },
                new Cliente
                {
                    Nome = "Felipe Augusto",
                    CEP = "12345678",
                    Cidade = "Rio de Janeiro",
                    Estado = "RJ",
                    Telefone = "21999999999"
                }};

            using var db = new Data.ApplicationContext();

            db.AddRange(listaCliente);

            var registros = db.SaveChanges(); // só insere 1x, pois monitora a instancia

            Console.WriteLine($"Total de registros inseridos: {registros}");
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1321354336643",
                Valor = 10m,
                TipoProduto = ValueObject.TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new Data.ApplicationContext();
            db.Produtos.Add(produto);
            //ou
            db.Set<Produto>().Add(produto);
            //ou
            db.Entry<Produto>(produto).State = EntityState.Added;
            //ou
            db.Add(produto);

            var registros = db.SaveChanges(); // só insere 1x, pois monitora a instancia

            Console.WriteLine($"Total de registros inseridos: {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new Data.ApplicationContext();
            var consultarPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Clientes.Where(p => p.Id > 0).ToList();
            var consultaPorMetodo2 = db.Clientes.AsNoTracking().Where(p => p.Id > 0).ToList(); //Apenas o metodo find usa o cache do track para obter os objetos

            foreach (var item in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando cliente id {item.Id}");

                db.Clientes.Find(item.Id); //PK pode utilizar do cache se a consulta nao tiver o AsNoTracking
                db.Clientes.FirstOrDefault(predicate => predicate.Id == item.Id); // nunca se utiliza de cache
            }
        }
    }
}

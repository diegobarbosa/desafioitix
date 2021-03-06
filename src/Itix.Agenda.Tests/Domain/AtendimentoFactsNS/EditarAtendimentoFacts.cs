﻿using Itix.Agenda.Core.Agenda;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Itix.Agenda.Core.Infra.Utils;
using NSubstitute;
using Itix.Agenda.Core.Domain.Pacientes;

namespace Itix.Agenda.Tests.Agenda.AtendimentoFactsNS
{
    public class EditarAtendimentoFacts : AtendimentoFacts
    {
        protected Atendimento editarAtendimento;

        protected PeriodoFechado novoHorario;

        public EditarAtendimentoFacts()
        {
            editarAtendimento = new Atendimento(atendimentoRepo, timeProvider, horario, paciente, observacao);
            editarAtendimento.SetProperty(x => x.IdAtendimento, 100);

            novoHorario = new PeriodoFechado(DateTime.Parse("2018-07-01 09:00"), DateTime.Parse("2018-07-01 09:40"));
        }

        [Fact]
        void tem_sucesso()
        {

            //ACT
            editarAtendimento.Alterar(atendimentoRepo, timeProvider, novoHorario, paciente, observacao);

            //ASSERT
            editarAtendimento.Horario.ShouldBe(novoHorario);

            editarAtendimento.Paciente.ShouldBe(paciente);

            editarAtendimento.Observacao.ShouldBe(observacao);
        }

        public class Falha_quando : EditarAtendimentoFacts
        {

            [Fact]
            void atendimentoRepo_null()
            {
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(null, timeProvider, novoHorario, paciente, observacao));

                ex.Message.ShouldBe("atendimentoRepo é null");
            }

            [Fact]
            void timeProvider_null()
            {
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(atendimentoRepo, null, novoHorario, paciente, observacao));

                ex.Message.ShouldBe("timeProvider é null");
            }

            [Fact]
            void horario_null()
            {
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(atendimentoRepo, timeProvider, null, paciente, observacao));

                ex.Message.ShouldBe("horario é null");
            }

            [Fact]
            void horario_eh_no_passado()
            {
                //Arrange
                //hoje = DateTime.Parse("01/05/2018 07:00");
                novoHorario = new PeriodoFechado(DateTime.Parse("2018-05-01 06:59"), DateTime.Parse("2018-05-01 07:30"));

                //ACT
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(atendimentoRepo, timeProvider, novoHorario, paciente, observacao));


                //ASSERT
                ex.Message.ShouldBe("Informe um Horário com data maior ou igual a de agora");
            }



            [Fact]
            void paciente_nao_informado()
            {
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(atendimentoRepo, timeProvider, novoHorario, null, observacao));

                ex.Message.ShouldBe("Informe o Paciente");
            }



            [Fact]
            void observacao_mais_de_500_caracteres()
            {
                //ARRANGE
                observacao = new string('p', 501);

                //ACT
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(atendimentoRepo, timeProvider, novoHorario, paciente, observacao));

                //ASSERT
                ex.Message.ShouldBe("Observação deve ter no máximo 500 caracteres");
            }


            [Fact]
            void horario_esta_indisponivel()
            {
                //ARRANGE
                var atendimentos = new List<Atendimento> { Substitute.For<Atendimento>() };
                atendimentoRepo.ExisteColisaoComOHorario(novoHorario, 100).Returns(atendimentos);//SIM

                //ACT
                var ex = Should.Throw<DomainException>(() => editarAtendimento.Alterar(atendimentoRepo, timeProvider, novoHorario, paciente, observacao));


                //ASSERT
                ex.Message.ShouldBe("Já existe Atendimento marcado para o Horário informado");
            }


        }



    }
}

# LudoLoca 🎲

[![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)](https://github.com/filipechgs/LodoLoca)
[![Licença](https://img.shields.io/badge/Licença-Educacional-blue)](LICENSE)

> Plataforma de compartilhamento de jogos de tabuleiro que promove o consumo consciente e a economia circular.

**LudoLoca** é um projeto acadêmico desenvolvido durante a disciplina de Projeto Integrador do curso de Análise e Desenvolvimento de Sistemas (SENAC - EAD). Nossa proposta é facilitar o aluguel de jogos de tabuleiro entre usuários, democratizando o acesso ao lazer e incentivando o consumo consciente.

## O que tem neste repositório
- "/Docs" - Contendo a documentação do projeto
- "/client" - Contendo a base em html para as views
- "/client/Rotas.md" - Lista dos endpoints da aplicação

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Instalação](#-instalação)
- [Como Contribuir](#-como-contribuir)
- [Licença](#-licença)

## 🎯 Sobre o Projeto

### Objetivo
Conectar pessoas que possuem jogos de tabuleiro ociosos com aquelas que desejam experimentá-los, criando uma comunidade colaborativa e sustentável.

### Público-Alvo
- 🎮 Jogadores casuais e entusiastas
- 👨‍👩‍👧‍👦 Famílias em busca de atividades educativas
- 👩‍🏫 Educadores e professores
- 👥 Grupos de amigos interessados em jogos de tabuleiro

### Personas Principais
1. **Lucas Fernandes**
   - Designer freelancer
   - Jogador casual
   - Busca variedade de jogos sem alto investimento

2. **Sandra Aparecida**
   - Professora
   - Mãe
   - Interessada em jogos educativos para desenvolvimento infantil

### Requisitos do usuário

1. Segurança e confiança nas transações
   - Nova usuária é criada apenas com a indicação de uma usuária já esistente (sugestão)
   - Usuária deve ser maior de 18 anos
   - Cadastrar apenas os dados necessários para viabilizar a transação: nome, email, cpf, idade, cidade, estado
   - Visualização da árvore de indicação do usuário 
   - Um usuário pode bloquear o recebimento de mensagens de outro usuário (sugestão)
   - Ranqueamento dos usuários feito pelas pessoas que negociam com ele
   - O chat para as transações ocorre através da plataforma do aplicativo (sugestão)
   - A pessoa é notificada, pelo app mobile, por email, ou por whatsapp, quando algem quer Conversar com ela (sugestão)
   - O aplicativo não se responsabilisa pelas transações.
   - Histórico dos jogos que pegou emprestado e dos que emprestou (sugestão)
   - Quem empresta, tem um inventário dos jogos que possui, com fotos e descrição atualizada do estado de conservação. (sugestão)
   - Quem pega emprestado tira foto do estado do jogo no momento em que recebeu. (sugestão)
   - O valor cobrado por emprestimo é transicionado diratamente entre os usuários

2. Busca, cadastro e informações sobre jogos na plataforma
   - O usuário pode buscar o jogo por nome, tipo, idade, quantidade de jogadores
   - O usuário pode cadastrar um jogo que não encontrou na plataforma
   - Pode adicionar a sua opinião a um jogo que ja tenha jogado ou explicar como o jogo funciona
   - A página do jogo exibe uma lista ranqueada de usuários próximos emprestando-o.

## ✨ Requisitos funcionais

- 👤 Sistema completo de cadastro e autenticação de usuários
- 🎲 Catálogo de geral jogos com filtros avançados
  - Categoria
  - Faixa etária
  - Avaliação
  - Preço
- 📝 Gestão de anúncios (criação, edição, remoção)
- 💬 Chat integrado entre locador e locatário
- 🔔 Sistema de notificações em tempo real
- ⭐ Avaliações e reviews de jogos e usuários

## Requisitos não funcionais

- 📱 Interface responsiva e acessível
- 📱 Mobile e web

## Limitações
- Deve ter baixo custo de implementação
- Deve usar tecnologias que a equipe ja tenha familiaridade ou com curva de aprendizagem fácil.

## 🛠️ Tecnologias utilizadas

### Frontend
- Flutter (multiplataforma)
  - iOS
  - Android
  - Web

### Backend
- API RESTful
- Sistema de autenticação JWT
- PostgreSQL (ou bd não relacional?)
- Websockets para chat em tempo real

### Outros Serviços
- Sistema de notificações push
- Serviço de e-mail transacional
- Mecanismo de busca avançada
- CDN para imagens

## 🗂️ Estrutura do Projeto

```
/
├── fase_1/          # Documentação inicial e visão do produto
├── docs/            # Diagramas, fluxogramas e documentação técnica
└── prototipos/      # Wireframes e protótipos de interface
```

## 🚀 Instalação

> ⚠️ Em desenvolvimento

Instruções detalhadas de instalação serão disponibilizadas em breve.

## 👥 Como Contribuir

1. 🍴 Faça um fork do projeto
2. 🔧 Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. 💻 Desenvolva sua feature
4. ✅ Faça commit das mudanças (`git commit -m 'Add: AmazingFeature'`)
5. 📤 Faça push para a branch (`git push origin feature/AmazingFeature`)
6. 🔄 Abra um Pull Request

## 📄 Licença

Este projeto é de uso educacional, desenvolvido exclusivamente para fins acadêmicos no curso de Análise e Desenvolvimento de Sistemas – SENAC.

---

**Desenvolvido com 💙 pelo Grupo 8 – Projeto Integrador II (2024)**

[🔝 Voltar ao topo](#ludoloca-)
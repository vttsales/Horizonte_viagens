# Preços: método e referências

Consulta de referências públicas feita em novembro de 2026 durante esta implementação. São referências de planejamento por rota/categoria e médias de hospedagem; não são cotações vinculantes para datas exatas. O pacote nunca deve ser anunciado como preço final antes da confirmação pela agência. A data de um festival precisa ser conferida para o ano da viagem.

## Composição inicial (reais)

| Destino | Transporte por adulto | Diária por quarto | Noites | Experiências por adulto | Total de 2 adultos |
|---|---:|---:|---:|---:|---:|
| Paris | 17.000 | 3.900 | 14 | 1.800 | 92.200 |
| Tóquio | 29.500 | 2.600 | 19 | 5.000 | 118.400 |
| Nova York | 19.500 | 4.600 | 9 | 2.800 | 86.000 |
| Porto Seguro | 1.200 | 450 | 9 | 600 | 7.650 |
| Ubatuba | 300 | 550 | 6 | 350 | 4.600 |
| Florianópolis | 850 | 470 | 7 | 500 | 5.990 |

Transporte e hospedagem foram arredondados a partir das referências abaixo. Experiências são **provisões de orçamento escolhidas para planejamento**, não preços verificados de ingressos. O total inclui somente os componentes cadastrados; não comprova taxas, disponibilidade ou contratação de fornecedor. O admin deve incorporar todos os custos aplicáveis ao confirmar a oferta.

Fórmula: `(transporte + experiências) × adultos + diária × noites × teto(adultos / 2)`. Os preços são em BRL; não há cotação cambial dinâmica. Até seis adultos; tarifas infantis, múltiplas origens e datas livres não foram implementadas. Uma saída tem data definida pelo combo.

## Referências consultadas

- Paris: [aéreo executivo](https://www.kayak.com.br/voos/Classe-executiva-Sao-Paulo-SAO/Paris-Charles-de-Gaulle-CDG.bc.ksp) e [hotéis 5 estrelas](https://www.kayak.com.br/Paris-Hoteis_5-estrelas.Hfs.36014.hotel.ksp).
- Tóquio: [aéreo executivo](https://www.kayak.com.br/voos/Classe-executiva-Sao-Paulo-Guarulhos-GRU/Toquio-TYO.bc.ksp) e [hotéis](https://www.momondo.com.br/hoteis/toquio).
- Nova York: [aéreo executivo](https://www.kayak.com.br/voos/Classe-executiva-Sao-Paulo-SAO/Nova-York-NYC.bc.ksp) e [hotéis](https://www.kayak.com.br/Nova-York-Hoteis.15830.hotel.ksp).
- Porto Seguro: [aéreo](https://www.kayak.com.br/voos/LATAM-Airlines-Sao-Paulo-SAO/Porto-Seguro-BPS.b.LA.ksp) e [hotéis](https://www.kayak.com.br/Porto-Seguro-Hoteis.47426.hotel.ksp).
- Ubatuba: [rodoviário São Paulo–Ubatuba](https://www.clickbus.com.br/passagem/sao-paulo-sp-para-ubatuba-sp) e [hospedagem 3 estrelas](https://br.trip.com/hotels/star3/city/br/ubatuba.html).
- Florianópolis: [aéreo](https://www.latamairlines.com/br/pt/destinos/voos-de-sao-paulo-para-florianopolis) e [hotéis](https://www.kayak.com.br/Florianopolis-Hoteis.29399.hotel.ksp).

## Eventos

- Desfiles oficiais da Paris Fashion Week dependem de convite; não prometer ingresso público: [FHCM](https://www.fhcm.paris/en/news/pfwr-fraudulent-solicitations).
- A celebração pública de Réveillon em Times Square é gratuita e não garante lugar: [FAQ oficial](https://www.timessquarenyc.org/nye/nye-faq).
- Hanami é uma experiência sazonal, não um único festival com ingresso universal. Locais, floração, passeios e eventuais entradas precisam de confirmação.
- Calendários de Carnaval, surfe e Fenaostra permanecem a confirmar. O sistema não inventa datas ou ingresso garantido.

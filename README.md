# Energy API

.NET 10 API over the UK [Carbon Intensity API](https://carbon-intensity.github.io/api-definitions/). Two endpoints: 3-day generation mix, and the optimal EV charging window over the next two days.

`dotnet run` (port 5197) · `dotnet test`

## Links

- **Live application:** https://energy-mix-frontend-0ru0.onrender.com/
- **Frontend repository:** https://github.com/kacpersmaga/energy-mix-frontend

## Design decisions

- **Two endpoints fetch independently.** `/chwindow` pulls fresh data rather than reusing `/mix`, so both always reflect the latest forecast.
- **`/mix` range is `today .. today+3`.** The API's `to` is exclusive, so `+3` is needed to fully cover the third day.
- **Ties go to the earliest window.** The search uses strict `>`, so the first maximum wins. The spec doesn't define this; earliest felt most useful.
- **Everything is UTC.** No BST/GMT adjustment — that matches the source API's contract.
- **Clean fuels:** biomass, nuclear, hydro, wind, solar (centralized in `EnergyConstants`).

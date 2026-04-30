Start-Process dotnet -ArgumentList "watch", "run", "--project", "./backend/api/", "--urls", "http://localhost:5000;https://localhost:5001"
Start-Process dotnet -ArgumentList "watch", "run", "--project", "./blazor/blazor/", "--urls", "http://localhost:5050;https://localhost:5051"

#!/usr/bin/env sh

dotnet watch run --project ./backend/api/ --urls "http://localhost:5000" &
dotnet watch run --project ./blazor/blazor/ --urls "http://localhost:5001" &


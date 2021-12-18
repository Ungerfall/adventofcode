# Advent of Code 2021

### azure functions cli commands

login before execution other commands
` bash
az login
`

create new function
` bash
func new --template "Blob Trigger" --name AdventOfCode2021_Day14_Part1
`

publish to Azure
` bash
func azure functionapp publish "fapp-ungerfall-adventofcode-2021"
`

$ErrorActionPreference = 'Stop'
$before = Join-Path $PSScriptRoot '_before'
$outDir = $PSScriptRoot

function Transform-ProcText([string]$text) {
    if ([string]::IsNullOrWhiteSpace($text)) { return $text }

    $text = $text -replace '(?i)\bCREATE\s+PROC(?:EDURE)?\b', 'CREATE OR ALTER PROC'

    # SupplierProjects: date-truncated compare -> full datetime (sp.enddate lowercase included)
    $text = [regex]::Replace($text,
        '(?i)convert\s*\(\s*date\s*,\s*((?:sp\.)?)StartDate\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s*(?:and|AND)\s*convert\s*\(\s*date\s*,\s*((?:sp\.)?)(?:EndDate|enddate)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        {
            param($m)
            $p1 = $m.Groups[1].Value
            $p2 = if ($m.Groups[2].Value) { $m.Groups[2].Value } else { $p1 }
            "${p1}StartDate >= @startdate AND ${p2}EndDate <= @enddate"
        })

    $text = [regex]::Replace($text,
        '(?i)\(\s*Convert\s*\(\s*date\s*,\s*((?:sp\.)?)StartDate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s*(?:and|AND)\s*\(\s*Convert\s*\(\s*date\s*,\s*((?:sp\.)?)(?:EndDate|enddate)\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        {
            param($m)
            $p1 = if ($m.Groups[1].Value) { $m.Groups[1].Value } else { '' }
            $p2 = if ($m.Groups[2].Value) { $m.Groups[2].Value } else { '' }
            "${p1}StartDate >= @startdate AND ${p2}EndDate <= @enddate"
        })

    $text = $text -replace '(?i)convert\s*\(\s*date\s*,\s*sp\.StartDate\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s*AND\s*convert\s*\(\s*date\s*,\s*enddate\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        'sp.StartDate >= @startdate AND sp.EndDate <= @enddate'

    # Projects: paired SDate/EDate filters (datetime params -> date-only on project columns)
    $projPair = 'CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)'
    $replacements = @(
        '(?i)convert\s*\(\s*datetime\s*,\s*\(\s*SDate\s*\)\s*\)\s*>=\s*@startdate\s+and\s+convert\s*\(\s*datetime\s*,\s*\(\s*EDate\s*\)\s*\)\s*<=\s*@enddate',
        '(?i)\(\s*Convert\s*\(\s*datetime\s*,\s*\(\s*SDate\s*\)\s*\)\s*\)\s*>=\s*@startdate\s+and\s+\(\s*Convert\s*\(\s*datetime\s*,\s*\(\s*EDate\s*\)\s*\)\s*\)\s*<=\s*@enddate',
        '(?i)\(\s*Convert\s*\(\s*datetime\s*,\s*SDate\s*\)\s*\)\s*>=\s*@startdate\s+and\s+\(\s*Convert\s*\(\s*datetime\s*,\s*EDate\s*\)\s*\)\s*<=\s*@enddate',
        '(?i)Convert\s*\(\s*datetime\s*,\s*SDate\s*\)\s*>=\s*@startdate\s+and\s+Convert\s*\(\s*datetime\s*,\s*EDate\s*\)\s*<=\s*@enddate',
        '(?i)\(\s*convert\s*\(\s*date\s*,\s*SDate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+\(\s*convert\s*\(\s*date\s*,\s*EDate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)\(\s*Convert\s*\(\s*date\s*,\s*SDate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+\(\s*Convert\s*\(\s*date\s*,\s*EDate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)convert\s*\(\s*date\s*,\s*SDate\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+convert\s*\(\s*date\s*,\s*EDate\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)Convert\s*\(\s*date\s*,\s*\(\s*SDate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+Convert\s*\(\s*date\s*,\s*\(\s*EDate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)Convert\s*\(\s*date\s*,\s*\(\s*SDate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+Convert\s*\(\s*date\s*,\s*\(\s*EDate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)\(\s*Convert\s*\(\s*date\s*,\s*SDate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+\(\s*Convert\s*\(\s*date\s*,\s*EDate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)\(\s*Convert\s*\(\s*date\s*,\s*sdate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+\(\s*Convert\s*\(\s*date\s*,\s*edate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        '(?i)\(\s*convert\s*\(\s*date\s*,\s*sdate\s*\)\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+\(\s*convert\s*\(\s*date\s*,\s*edate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)'
    )
    foreach ($pat in $replacements) {
        $text = [regex]::Replace($text, $pat, $projPair)
    }

    # Single-sided project date compares (client filter block)
    $text = [regex]::Replace($text,
        '(?i)Convert\s*\(\s*date\s*,\s*SDate\s*\)\s*>=\s*convert\s*\(\s*date\s*,\s*@startdate\s*\)\s+and\s+\(\s*Convert\s*\(\s*date\s*,\s*EDate\s*\)\s*\)\s*<=\s*convert\s*\(\s*date\s*,\s*@enddate\s*\)',
        $projPair)

    return $text
}

$procs = @(
    'usp_ReportManagement_bk',
    'usp_CalculateRevenue_BK',
    'respondants',
    'remainingtabled',
    'ManagerWiseReport_BK'
)

foreach ($name in $procs) {
    $src = Join-Path $before "$name.sql"
    $raw = Get-Content -Raw -Path $src
    $raw = $raw.Trim()
    if ($raw -match '^\s*\d+\s') { $raw = ($raw -split "`n" | Select-Object -Skip 1) -join "`n" }
    $transformed = Transform-ProcText $raw
    $dest = Join-Path $outDir "$name.sql"
    Set-Content -Path $dest -Value $transformed -Encoding UTF8
    Write-Host "Wrote $dest"
}

Write-Host 'Done.'

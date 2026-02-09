$path = 'ZyphraTrades\MainWindow.xaml'
$content = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
$rep = [char]0xFFFD

# More emoji fixes
$save = [char]::ConvertFromUtf32(0x1F4BE)
$swap = [char]::ConvertFromUtf32(0x1F500)
$trash = [char]::ConvertFromUtf32(0x1F5D1)
$content = $content.Replace('?? Guardar"', ($save + ' Guardar"'))
$content = $content.Replace('?? Toggle', ($swap + ' Toggle'))
$content = $content.Replace('?? Guardar Preferencias', ($save + ' Guardar Preferencias'))

# More accent fixes - uppercase patterns with U+FFFD
$fixes2 = @()
$fixes2 += , @(('B' + $rep + 'SICA'), ('B' + [char]0xC1 + 'SICA'))
$fixes2 += , @(('EMOCI' + $rep + 'N'), ('EMOCI' + [char]0xD3 + 'N'))
$fixes2 += , @(('PUNTUACI' + $rep + 'N'), ('PUNTUACI' + [char]0xD3 + 'N'))
$fixes2 += , @(('LECCI' + $rep + 'N'), ('LECCI' + [char]0xD3 + 'N'))
$fixes2 += , @(('OBSERVACI' + $rep + 'N'), ('OBSERVACI' + [char]0xD3 + 'N'))
$fixes2 += , @(('Estad' + $rep + 'sticas'), ('Estad' + [char]0xED + 'sticas'))
$fixes2 += , @((' ' + $rep + ' '), (' ' + [char]0xB7 + ' '))
$fixes2 += , @(('M' + $rep + 'X'), ('M' + [char]0xC1 + 'X'))
$fixes2 += , @(('M' + $rep + 'x'), ('M' + [char]0xE1 + 'x'))
$fixes2 += , @(('P' + $rep + 'RD'), ('P' + [char]0xC9 + 'RD'))
$fixes2 += , @(('P' + $rep + 'rdida'), ('P' + [char]0xE9 + 'rdida'))
$fixes2 += , @(('Duraci' + $rep + 'n'), ('Duraci' + [char]0xF3 + 'n'))
$fixes2 += , @(('Din' + $rep + 'micos'), ('Din' + [char]0xE1 + 'micos'))
$fixes2 += , @(('autom' + $rep + 'ticamente'), ('autom' + [char]0xE1 + 'ticamente'))
$fixes2 += , @(('Categor' + $rep + 'a'), ('Categor' + [char]0xED + 'a'))
$fixes2 += , @(('Descripci' + $rep + 'n'), ('Descripci' + [char]0xF3 + 'n'))

foreach ($fix in $fixes2) {
    $content = $content.Replace($fix[0], $fix[1])
}

$utf8Bom = New-Object System.Text.UTF8Encoding($true)
[System.IO.File]::WriteAllText($path, $content, $utf8Bom)

$remaining = ([regex]::Matches($content, [string]$rep)).Count
Write-Host "Done. Remaining U+FFFD: $remaining"
$qqCount = ([regex]::Matches($content, '\?\?')).Count
Write-Host "Remaining ??: $qqCount"

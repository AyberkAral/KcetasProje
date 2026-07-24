$files = Get-ChildItem -Path "Controllers" -Filter "*.cs" -Recurse

foreach ($file in $files) {
    $content = [IO.File]::ReadAllText($file.FullName)
    $original = $content
    
    $content = [regex]::Replace($content, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.Where", "(await `$1).Where")
    $content = [regex]::Replace($content, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.OrderBy", "(await `$1).OrderBy")
    $content = [regex]::Replace($content, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.Select", "(await `$1).Select")
    $content = [regex]::Replace($content, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.FirstOrDefault", "(await `$1).FirstOrDefault")
    $content = [regex]::Replace($content, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.To", "(await `$1).To")

    $content = [regex]::Replace($content, "_aboneService\.Create\(", "await _aboneService.CreateAsync(")
    $content = [regex]::Replace($content, "_aboneService\.Update\(", "await _aboneService.UpdateAsync(")
    $content = [regex]::Replace($content, "_aboneService\.Delete\(", "await _aboneService.DeleteAsync(")
    $content = [regex]::Replace($content, "(?<!await\s*)_aboneService\.GetById\(", "await _aboneService.GetByIdAsync(")
    $content = [regex]::Replace($content, "(?<!await\s*)_aboneService\.GetAll\(", "await _aboneService.GetAllAsync(")
    
    $content = [regex]::Replace($content, "_auditLogService\.Ekle\(", "await _auditLogService.EkleAsync(")
    
    $content = [regex]::Replace($content, "_faturaService\.SimulasyonHesapla\(", "await _faturaService.SimulasyonHesaplaAsync(")
    
    $content = [regex]::Replace($content, "_kullaniciDeposu\.BulKullaniciAdiIle\(", "await _kullaniciDeposu.BulKullaniciAdiIleAsync(")
    
    $content = [regex]::Replace($content, "(?<!await\s*)_sozlesmeService\.GetAll\(", "await _sozlesmeService.GetAllAsync(")
    $content = [regex]::Replace($content, "(?<!await\s*)_tuketimNoktasiService\.GetAll\(", "await _tuketimNoktasiService.GetAllAsync(")
    
    $content = [regex]::Replace($content, "_isEmriService\.Ekle\(", "await _isEmriService.EkleAsync(")

    if ($content -cne $original) {
        Write-Host "Updated $($file.Name)"
        [IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
    }
}

 = Get-ChildItem -Path "Controllers" -Filter "*.cs" -Recurse

foreach ( in ) {
     = [IO.File]::ReadAllText(.FullName)
     = 
    
     = [regex]::Replace(, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.Where", "(await $1).Where")
     = [regex]::Replace(, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.OrderBy", "(await $1).OrderBy")
     = [regex]::Replace(, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.Select", "(await $1).Select")
     = [regex]::Replace(, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.FirstOrDefault", "(await $1).FirstOrDefault")
     = [regex]::Replace(, "await\s+(_[a-zA-Z0-9_]+\.GetAllAsync\(\))\s*\.To", "(await $1).To")

     = [regex]::Replace(, "_aboneService\.Create\(", "await _aboneService.CreateAsync(")
     = [regex]::Replace(, "_aboneService\.Update\(", "await _aboneService.UpdateAsync(")
     = [regex]::Replace(, "_aboneService\.Delete\(", "await _aboneService.DeleteAsync(")
     = [regex]::Replace(, "(?<!await\s*)_aboneService\.GetById\(", "await _aboneService.GetByIdAsync(")
     = [regex]::Replace(, "(?<!await\s*)_aboneService\.GetAll\(", "await _aboneService.GetAllAsync(")
    
     = [regex]::Replace(, "_auditLogService\.Ekle\(", "await _auditLogService.EkleAsync(")
    
     = [regex]::Replace(, "_faturaService\.SimulasyonHesapla\(", "await _faturaService.SimulasyonHesaplaAsync(")
    
     = [regex]::Replace(, "_kullaniciDeposu\.BulKullaniciAdiIle\(", "await _kullaniciDeposu.BulKullaniciAdiIleAsync(")
    
     = [regex]::Replace(, "(?<!await\s*)_sozlesmeService\.GetAll\(", "await _sozlesmeService.GetAllAsync(")
     = [regex]::Replace(, "(?<!await\s*)_tuketimNoktasiService\.GetAll\(", "await _tuketimNoktasiService.GetAllAsync(")
    
     = [regex]::Replace(, "_isEmriService\.Ekle\(", "await _isEmriService.EkleAsync(")

    if ( -cne ) {
        Write-Host "Updated "
        [IO.File]::WriteAllText(.FullName, , [System.Text.Encoding]::UTF8)
    }
}

 = Get-ChildItem -Path "Controllers" -Filter "*.cs" -Recurse

foreach ( in ) {
     = Get-Content .FullName -Raw
    
     =  -replace '_aboneService\.Create\(', 'await _aboneService.CreateAsync('
     =  -replace '_aboneService\.GetAll\(', '(await _aboneService.GetAllAsync())'
     =  -replace '_aboneService\.GetById\(', 'await _aboneService.GetByIdAsync('
     =  -replace '_aboneService\.Update\(', 'await _aboneService.UpdateAsync('
     =  -replace '_aboneService\.Delete\(', 'await _aboneService.DeleteAsync('
    
     =  -replace '_auditLogService\.Ekle\(', 'await _auditLogService.EkleAsync('
    
     =  -replace '_faturaService\.SimulasyonHesapla\(', 'await _faturaService.SimulasyonHesaplaAsync('
     =  -replace '_kullaniciDeposu\.BulKullaniciAdiIle\(', 'await _kullaniciDeposu.BulKullaniciAdiIleAsync('
    
     =  -replace '(?<!await\s*)_sozlesmeService\.GetAll\(', '(await _sozlesmeService.GetAllAsync()'
     =  -replace '(?<!await\s*)_tuketimNoktasiService\.GetAll\(', '(await _tuketimNoktasiService.GetAllAsync()'
    
     =  -replace '_isEmriService\.Ekle\(', 'await _isEmriService.EkleAsync('

    Set-Content -Path .FullName -Value  -Encoding UTF8
}

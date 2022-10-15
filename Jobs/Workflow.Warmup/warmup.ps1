
function CallApi ([string]$path,$hosts,$body,[string]$method){
	$body = ConvertTo-Json $body
   foreach($host1 in $hosts){
		$url = $path.Replace("{host}",$host1);
		Write-Host "Calling $url";
		Try
		{
            if ($method -eq 'Get'){
    			Invoke-RestMethod -Uri $url -Method Get  -ErrorAction Stop ;
            }else{
                Invoke-RestMethod -Uri $url -Method Post -Body $body  -ContentType "application/json"   -ErrorAction Stop ;

            }
		}
		Catch
		{
			$ErrorMessage = $_.Exception.Message
			Write-Host $ErrorMessage
		}	
	}
		
}


$systemId = "0E56E4FE722847BCBD4F2569E2C87E14";
$mainHost = "eapstgweb2";
#$hosts = @($mainHost);
$hosts = @($mainHost,"eapstgweb1");


$apiBase =  "http://{host}/Forms/FormsService.svc/json"
$body = ConvertTo-Json @{systems = @($systemId)}
$forms = Invoke-RestMethod -Uri "http://$mainHost/Forms/FormsService.svc/json/forms/systems/any" -Method Post -Body $body  -ContentType "application/json" 
foreach($form in $forms.data){
		
        $id = $form.id
		$url = "$apiBase/forms/$id/platforms/00000000000000000000000000000001/render";
		CallApi $url $hosts nil 'Get';
		$url = "$apiBase/forms/$id/platforms/00000000000000000000000000000002/render";
		CallApi $url $hosts nil 'Get';
}


$apiBase = "http://{host}/Lookups/LookupsService.svc/json";
$lookups = Invoke-RestMethod -Uri "http://$mainHost/Lookups/LookupsService.svc/json/lookups/systems/$systemId" -Method Get 
foreach($lookup in $lookups.data){
    $id = $lookup.lookupId;
    CallApi "http://{host}/Lookups/LookupsService.svc/json/lookups/$id/items" $hosts nil 'Get'
}





$apiBase =  "http://{host}/Workflows/WorkflowsService.svc/json"
$body = ConvertTo-Json @{SystemIds = @($systemId)}
$workflows = Invoke-RestMethod -Uri "http://$mainHost/Workflows/WorkflowsService.svc/json/workflows/systems" -Method Post -Body $body  -ContentType "application/json" 
foreach($workflow in $workflows.data){
         $name  = $workflow.name
        Write-Host "Loading workflow $name"
        $id = $workflow.workflowId
		
		$url = "$apiBase/workflows/$id/0/Warmup/start";
		CallApi $url $hosts @{parameters = "{}" } 'Post';
}




CallApi 'http://{host}/MappingMatrix/MappingMatrixService.svc/json/domainTypes' $hosts nil 'Get'
CallApi 'http://{host}/Localization/LocalizationService.svc/json/localsets/systems/D52589D60BF846D2BCE1BFFE5026B89F/titles' $hosts nil 'Get'
CallApi 'http://{host}/Localization/LocalizationService.svc/json/localsets/systems/08867038D3934BCA804CD4074735B260/titles' $hosts nil 'Get'

CallApi 'http://{host}/Services/ServicesService.svc/json/services/systems' $hosts @{SystemIds = @($systemId)} 'Post'
CallApi 'http://{host}/Vision/VisionService.svc/json/individual/sponsor/sponsorNo/12345' $hosts nil 'Get'

CallApi 'http://{host}/Channel/ChannelService.svc/json/users/username/test/available' $hosts nil 'Get'

CallApi 'http://{host}/Application/ApplicationService.svc/json/applications/status' $hosts nil 'Get'
CallApi "http://{host}/Systems/SystemsService.svc/json/systems/$systemId/config" $hosts nil 'Get'

CallApi 'http://{host}/SMS/SMSService.svc/json/sms' $hosts @{mobileNo="0567993844";systemId="Warmup";message="Application Startup" } 'Post';
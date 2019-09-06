package Tailwind.Traders.Stock.Api.config;

import org.springframework.context.annotation.Bean;
import org.springframework.util.StringUtils;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import com.microsoft.applicationinsights.TelemetryConfiguration;

@Configuration
public class AppInsightsConfig { 
	//Initialize AI TelemetryConfiguration via Spring Beans
    @Bean
    public String telemetryConfig(@Value("${azure.application-insights.instrumentation-key}") String telemetryKey) {
        if (telemetryKey != null && !StringUtils.isEmpty(telemetryKey)) {
        	TelemetryConfiguration.getActive().setInstrumentationKey(telemetryKey);
        }
        return telemetryKey;
    }
}

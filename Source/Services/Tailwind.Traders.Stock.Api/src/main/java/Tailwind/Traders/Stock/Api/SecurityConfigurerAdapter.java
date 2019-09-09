package Tailwind.Traders.Stock.Api;

import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter;

@Configuration
@EnableWebSecurity
public class SecurityConfigurerAdapter extends  WebSecurityConfigurerAdapter {
	@Override
	protected void configure(HttpSecurity http) throws Exception{
		http.csrf().disable().cors().and()
			.authorizeRequests()
				.antMatchers("/v1/stock/*", "/", "/swagger-ui.html").permitAll()
				.antMatchers("/").permitAll()
				.antMatchers("/swagger-ui.html").permitAll();
				//.anyRequest().authenticated();
	}
}

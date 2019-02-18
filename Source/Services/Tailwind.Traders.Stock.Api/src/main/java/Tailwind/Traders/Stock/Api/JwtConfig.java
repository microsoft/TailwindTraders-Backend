package Tailwind.Traders.Stock.Api;

import org.springframework.beans.factory.annotation.Value;

public class JwtConfig {

	@Value("${security.secretkey}")
	private String secretKey;
	
	@Value("${security.issuer}")
	private String tokenIssuer;
	
	public String getSecretKey() {
		return secretKey;
	}
	
	public String getIssuer() {
		return tokenIssuer;
	}
}

package Tailwind.Traders.Stock.Api;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;

import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.web.authentication.www.BasicAuthenticationFilter;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.JwtParser;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.MalformedJwtException;
import io.jsonwebtoken.SignatureException;
import io.jsonwebtoken.UnsupportedJwtException;

public class JWTAuthorizationFilter extends BasicAuthenticationFilter {

	private static String HEADER_AUTHORIZATION_KEY =  "Authorization";
	private static String TOKEN_BEARER_PREFIX =  "Bearer ";
	private JwtConfig config;
	
	private final Logger log = LogManager.getLogger();

	public JWTAuthorizationFilter(AuthenticationManager authManager) {
		super(authManager);
	}

	public JWTAuthorizationFilter(AuthenticationManager authenticationManager, JwtConfig jwtConfig) {
		super(authenticationManager);
		config = jwtConfig;
	}

	@Override
	protected void doFilterInternal(HttpServletRequest req, HttpServletResponse res, FilterChain chain)
			throws IOException, ServletException {		
		String header = req.getHeader(HEADER_AUTHORIZATION_KEY);
		if (header == null || !header.startsWith(TOKEN_BEARER_PREFIX)) {
			chain.doFilter(req, res);
			return;
		}
			
		String token =  header.replace(TOKEN_BEARER_PREFIX, "");
		if(token == null) {
			chain.doFilter(req, res);
			return;
		}
		
		if(IsValidToken(token)) {
			String user = GetUserFromToken(token);
			UsernamePasswordAuthenticationToken authentication = new UsernamePasswordAuthenticationToken(user, null, new ArrayList<>());
			
			SecurityContextHolder.getContext().setAuthentication(authentication);
			chain.doFilter(req, res);			
		}
		else {
			res.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
		}	
	}
	
	
	private String GetUserFromToken(String token) throws UnsupportedJwtException, MalformedJwtException, SignatureException, IllegalArgumentException, UnsupportedEncodingException {
		Claims claims = Jwts.parser()
				.setSigningKey(config.getSecretKey().getBytes("UTF-8"))
				.parseClaimsJws(token)
				.getBody();
		
		return (String) claims.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
	}

	private boolean IsValidToken(String token) throws UnsupportedEncodingException {

		byte[] secretKey;

		try {
			secretKey = config.getSecretKey().getBytes("UTF-8");
		} catch (UnsupportedEncodingException encodingException) {
			log.error("Invalid secret key configured. UnsupportedEncoding exception {}", encodingException.getMessage());
			throw encodingException;
		}

		JwtParser parser = Jwts.parser().setSigningKey(secretKey);

		try {
			if(parser.isSigned(token)) {
				String claimIssuer = parser.parseClaimsJws(token).getBody().getIssuer();
				String validIssuer = config.getIssuer();

				return claimIssuer.equals(validIssuer); 
			}						 
		}catch(SignatureException signatureEx)
		{
			log.error("The token signature is invalid. SignaturException {}", signatureEx.getMessage());
		}
		return false;
	}
}
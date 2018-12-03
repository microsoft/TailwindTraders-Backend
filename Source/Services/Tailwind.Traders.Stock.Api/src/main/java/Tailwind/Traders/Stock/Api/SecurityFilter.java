package Tailwind.Traders.Stock.Api;

import java.io.IOException;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.core.annotation.Order;
import org.springframework.stereotype.Component;

@Component
@Order(1)
public class SecurityFilter implements Filter {
	private final Logger log = LogManager.getLogger();
	
	@Override
	public void init(FilterConfig filterConfig) throws ServletException {
	}

	@Override
	public void doFilter(ServletRequest request, javax.servlet.ServletResponse response, FilterChain chain)
			throws IOException, ServletException {
		HttpServletRequest req = (HttpServletRequest) request;
		HttpServletResponse res = (HttpServletResponse) response;
		
		String headerValue = req.getHeader("Authorization");
		
		if(headerValue != null) {
			String[] split = headerValue.split(" ");
			
			if(split.length > 1 && split[0].equals("Email") && split[1].length() > 0) {
				log.info("Starting a transaction for req : {}", req.getRequestURI());
				
				chain.doFilter(request, response);
			}
			else {
				res.sendError(401, "Invalid Authorization Header");
			}
		}else {
			res.sendError(403, "No Authorization Header Provided");
		}
	}

	@Override
	public void destroy() {		
	}
}